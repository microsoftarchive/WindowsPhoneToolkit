using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Resources;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.IO;
using System.Resources;

namespace SilverlightApp
{
    public class Picture
    {
        private static Dictionary<string, Picture> _pictures = null;

        public string Name { get; set; }
        public BitmapSource Bitmap { get; set; }

        private static IDictionary<string,Picture> LoadPictures()
        {
            if (null == _pictures)
            {
                _pictures = new Dictionary<string,Picture>();

                lock (_pictures)
                {
                    if (null != _pictures)
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        string[] resources = assembly.GetManifestResourceNames();
                        foreach (string res in resources)
                        {
                            if (!res.Contains("resources"))
                            {
                                using (Stream stream = assembly.GetManifestResourceStream(res))
                                {
                                    BitmapImage bitmap = new BitmapImage();
                                    bitmap.SetSource(stream);

                                    string[] split = res.Split('.');
                                    string name = split[2];

                                    _pictures.Add(name, new Picture() { Name = name, Bitmap = bitmap });
                                }
                            }
                        }
                    }
                }
            }

            return _pictures;
        }

        public static IList<Picture> GetPictures(int max)
        {
            LoadPictures();
            List<Picture> pictures = new List<Picture>();

            int num = 0;
            foreach (var pic in _pictures)
            {
                pictures.Add(pic.Value);
                if (++num >= max) break;
            }

            return pictures;
        }

        public static Picture Get(string name)
        {
            LoadPictures();

            return _pictures[name];
        }
    }

}
