using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;

namespace Weather
{
    [XmlRoot("search_api")]
    public class AreaSearchResult
    {
        [XmlElement("result")]
        public List<WeatherArea> Areas { get; set; }
    }

    public delegate void AreaSearchAsyncHandler(object sender, AreaSearchAsyncEventArgs e);
    public class AreaSearchAsyncEventArgs : EventArgs
    {
        public EventWaitHandle WaitHandle;
        public List<WeatherArea> Areas;
        public Exception Error;
        public object UserState;
    }

    public class AreaSearchService
    {
        private const string _urlformat = "http://www.worldweatheronline.com/feed/search.ashx?key={0}&query={1}&num_of_results={2}&format=xml";
        private static XmlSerializer _serializer = new XmlSerializer(typeof(AreaSearchResult));

        public event AreaSearchAsyncHandler SearchAsyncCompleted;
        private WebClient _wc;

        public static string ApiKey { get; set; }
        public string GetUrl(string query, int results = 3)
        {
            // invalid api key
            // go get one from http://www.worldweatheronline.com/register.aspx
            if (string.IsNullOrEmpty(ApiKey))
                throw new InvalidOperationException("ApiKey is invalid.");

            // return the formatted url
            return string.Format(CultureInfo.InvariantCulture, _urlformat, ApiKey, HttpUtility.UrlEncode(query), results);
        }

        public List<WeatherArea> Search(string query)
        {
            // get url string for this area
            string url = GetUrl(query);

            // new sync handle for wait operation
            using (EventWaitHandle wait = new ManualResetEvent(false))
            {
                // event complete args
                AreaSearchAsyncEventArgs args = new AreaSearchAsyncEventArgs() { WaitHandle = wait };

                WebClient wc = new WebClient();
                wc.OpenReadCompleted += new OpenReadCompletedEventHandler(AreaSearchService_LoadCompleted);
                wc.OpenReadAsync(new Uri(url, UriKind.Absolute), args);

                // wait for the call to complete
                wait.WaitOne();

                // return
                wait.Close();
                return args.Areas;
            }
        }

        private void AreaSearchService_LoadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            // restore state
            AreaSearchAsyncEventArgs args = (AreaSearchAsyncEventArgs)e.UserState;

            // call succeeded
            if (null == e.Error)
            {
                // deserialize weather information
                XmlReader reader = XmlReader.Create(e.Result);
                AreaSearchResult result = (AreaSearchResult)_serializer.Deserialize(reader);
                args.Areas = result.Areas;
                reader.Close();

                args.WaitHandle.Set();
                return;
            }

            // call failed
            args.Error = e.Error;
            args.WaitHandle.Set();
        }

        public void SearchAsync(string query)
        {
            SearchAsync(query, null);
        }

        public void SearchAsync(string query, object userState)
        {
            // get url string for this area
            string url = GetUrl(query);

            // event complete args
            AreaSearchAsyncEventArgs args = new AreaSearchAsyncEventArgs() { UserState = userState };

            // make the async call
            _wc = new WebClient();
            _wc.OpenReadCompleted += new OpenReadCompletedEventHandler(AreaSearchService_LoadAsyncCompleted);
            _wc.OpenReadAsync(new Uri(url, UriKind.Absolute), args);
        }

        public void CancelAsync()
        {
            _wc.CancelAsync();
            _wc = null;
        }

        private void AreaSearchService_LoadAsyncCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            // restore state
            AreaSearchAsyncEventArgs args = (AreaSearchAsyncEventArgs)e.UserState;

            // call succeeded
            if (null == e.Error)
            {
                // deserialize weather information
                XmlReader reader = XmlReader.Create(e.Result);
                AreaSearchResult result = (AreaSearchResult)_serializer.Deserialize(reader);
                args.Areas = result.Areas;
                reader.Close();
            }

            // pass exception data
            args.Error = e.Error;

            // notify callers
            if (null != this.SearchAsyncCompleted)
                this.SearchAsyncCompleted(this, args);
        }
    }
}
