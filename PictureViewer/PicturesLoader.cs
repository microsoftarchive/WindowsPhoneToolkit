using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Media.Imaging;
using System.Threading;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;

namespace PictureViewer
{
    public delegate void PicturesLoaderCompletedEventHandler(object sender, PicturesLoaderCompletedEventArgs e);
    public class PicturesLoaderCompletedEventArgs : EventArgs
    {
    }

    public delegate void PicturesLoaderProgressEventHandler(object sender, PicturesLoaderProgressEventArgs e);
    public class PicturesLoaderProgressEventArgs : EventArgs
    {
        public int Num;
        public int Max;
    }

    public class PicturesLoader
    {
        private static readonly string BingArchiveUrl = "http://www.bing.com/HPImageArchive.aspx?format=xml&idx={0}&n=1";
        private static readonly Uri BingUri = new Uri("http://www.bing.com", UriKind.Absolute);
        private static readonly int BingImageCount = 25;
        private static readonly string PicturesFolder = "pictures";
        private static readonly string PicturesConfig = Path.Combine(PicturesFolder, "pictures.xml");

        public static event PicturesLoaderCompletedEventHandler LoadCompleted;
        public static event PicturesLoaderProgressEventHandler LoadProgress;

        private static Dictionary<string, Picture> _pictures = null;
        private static ManualResetEvent _event = new ManualResetEvent(false);

        public IList<Picture> Recent { get { return PicturesLoader.GetPictures(6); } }
        public IList<Picture> Samples { get { return PicturesLoader.GetPictures(); } }

        public static void LoadPictures()
        {
            if (null == _pictures)
            {
                _pictures = new Dictionary<string, Picture>();

                // load pictures
                // start with isolated storage
                if (!LoadPicturesFromStorage())
                {
                    // fall back to web
                    LoadPicturesFromWeb();
                }
            }
        }

        public static IList<Picture> GetPictures(int max = 0)
        {
            // not initialized yet
            // don't forget to call PicturesLoader.Load
            if (null == _pictures)
                return null;

            List<Picture> pictures = new List<Picture>();

            // count not specified
            // return all
            if (max == 0) max = _pictures.Count;

            // build pictures list
            int num = 0;
            foreach (var pic in _pictures)
            {
                pictures.Add(pic.Value);
                if (++num >= max) break;
            }

            // done
            return pictures;
        }

        public static Picture Get(string name)
        {
            // not initialized yet
            // don't forget to call PicturesLoader.Load
            if (null == _pictures)
                return null;

            return _pictures[name];
        }

        #region IsolatedStorage
        private static void InitStorage(IsolatedStorageFile store, bool reset = false)
        {
            // reset storage ?
            if (reset) store.Remove();

            // create folder if not exists
            if (!store.DirectoryExists(PicturesFolder))
                store.CreateDirectory(PicturesFolder);
        }

        private static Stream LoadFromStorage(string url)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            return store.OpenFile(url, FileMode.Open);
        }

        private static void SaveToStorage(string url, Stream stream)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            InitStorage(store);

            // allocate buffer
            int lenght = (int)stream.Length;
            byte[] buffer = new byte[lenght];
            if (null != buffer)
            {
                // read from stream
                stream.Position = 0L;
                int offset = stream.Read(buffer, 0, lenght);

                // create a new file (overwrite existing)
                using (IsolatedStorageFileStream file = store.CreateFile(url))
                {
                    file.Write(buffer, 0, lenght);
                }
            }
        }

        private static void SaveToStorage(string url, string text)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            InitStorage(store);

            // create a new file (overwrite existing)
            using (IsolatedStorageFileStream file = store.CreateFile(url))
            {
                using (StreamWriter stream = new StreamWriter(file))
                    stream.Write(text);
            }
        }

        private static string GetConfigXml()
        {
            if (null == _pictures)
                return string.Empty;

            // build XML document
            XElement xml = new XElement("images");
            foreach (var image in _pictures)
            {
                Picture picture = image.Value;

                // xml
                xml.Add(new XElement("image",
                    new XElement("name", picture.Name),
                    new XElement("startdate", picture.Date),
                    new XElement("copyright", picture.Desc),
                    new XElement("url", picture.Url)
                    ));
            }

            return xml.ToString();
        }

        private static bool LoadPicturesFromStorage()
        {
            try
            {
                // load XML
                using (Stream file = LoadFromStorage(PicturesConfig))
                {
                    XDocument xml = XDocument.Load(file);
                    var images = from x in xml.Descendants("image")
                                 select new
                                 {
                                     Name = x.Element("name").Value,
                                     Date = x.Element("startdate").Value,
                                     Desc = x.Element("copyright").Value,
                                     Url = x.Element("url").Value
                                 };
                    foreach (var image in images)
                    {
                        Picture picture = new Picture()
                        {
                            Name = image.Name,
                            Date = image.Date,
                            Desc = image.Desc,
                            Url = image.Url,
                            Bitmap = new BitmapImage()
                        };
                        using (Stream stream = LoadFromStorage(picture.Url))
                            picture.Bitmap.SetSource(stream);

                        // add picture to dictionary
                        _pictures.Add(picture.Name, picture);
                    }

                    // notify we're done
                    if (null != LoadCompleted)
                        LoadCompleted(_pictures, new PicturesLoaderCompletedEventArgs());
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
#endregion IsolatedStorage

        #region WebClient
        private static bool LoadPicturesFromWeb()
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                // batch load all pictures using worker thread synchrounously
                for (int idx = 0; idx < BingImageCount; idx++)
                {
                    // reset event
                    _event.Reset();

                    // notify we're about to load a picture
                    if (null != LoadProgress)
                        LoadProgress(_pictures, new PicturesLoaderProgressEventArgs() { Num = idx + 1, Max = BingImageCount });

                    // load the picture asynchronously
                    Uri uri = new Uri(string.Format(BingArchiveUrl, idx), UriKind.Absolute);
                    WebClient wc = new WebClient();
                    wc.OpenReadCompleted += new OpenReadCompletedEventHandler(LoadPictureCompleted);
                    wc.OpenReadAsync(uri);

                    // block until async call is complete
                    _event.WaitOne();
                }

                // save pictures list
                SaveToStorage(PicturesConfig, GetConfigXml());

                // notify we're done
                if (null != LoadCompleted)
                    LoadCompleted(_pictures, new PicturesLoaderCompletedEventArgs());
            });

            return true;
        }

        private static void LoadPictureCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                using (Stream s = e.Result)
                {
                    XDocument xml = XDocument.Load(s);
                    var images = from x in xml.Descendants("image")
                                 select new
                                 {
                                     Date = x.Element("startdate").Value,
                                     Desc = x.Element("copyright").Value,
                                     Url = x.Element("url").Value
                                 };
                    var image = images.FirstOrDefault();
                    if (null != image)
                    {
                        Picture picture = new Picture()
                        {
                            Name = image.Url,
                            Date = image.Date,
                            Desc = image.Desc,
                            Url = image.Url,
                            Bitmap = new BitmapImage()
                        };

                        // parse name
                        // format : /xxx/yyy/zzz/ImageName_XX-XX1234567890.jpg
                        int start = image.Url.LastIndexOf('/') + 1;
                        int end = image.Url.IndexOf('_');
                        if ((start > 0) && (end > 0))
                            picture.Name = image.Url.Substring(start, end - start);

                        // load the image file asynchronously
                        WebClient wc = new WebClient();
                        wc.OpenReadCompleted += new OpenReadCompletedEventHandler(LoadBitmapCompleted);
                        wc.OpenReadAsync(new Uri(BingUri, image.Url), picture);

                        // done
                        return;
                    }
                }
            }

            // set event to unblock caller
            _event.Set();
        }

        private static void LoadBitmapCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Picture picture = e.UserState as Picture;
                if (null != picture)
                {
                    using (Stream stream = e.Result)
                    {
                        // fix url to storage path
                        picture.Url = Path.Combine(PicturesFolder, picture.Name + picture.Date);

                        // set bitmap source
                        picture.Bitmap.SetSource(stream);

                        // save to storage
                        SaveToStorage(picture.Url, stream);

                        // add picture to dictionary
                        _pictures.Add(picture.Name, picture);
                    }
                }
            }

            // set event to unblock caller
            _event.Set();
        }
    }
#endregion WebClient

}
