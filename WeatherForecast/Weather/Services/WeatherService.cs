using System;
using System.Net;
using System.Threading;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;

namespace Weather
{
    public delegate void WeatherLoadAsyncHandler(object sender, WeatherLoadAsyncEventArgs e);
    public class WeatherLoadAsyncEventArgs : EventArgs
    {
        internal EventWaitHandle WaitHandle;
        public WeatherData Weather;
        public Exception Error;
        public object UserState;
    }

    public class WeatherService
    {
        private const string _urlformat = "http://www.worldweatheronline.com/feed/weather.ashx?key={0}&lat={1}&lon={2}&num_of_days={3}&includeLocation=yes&format=xml";
        private static XmlSerializer _serializer = new XmlSerializer(typeof(WeatherData));

        public event WeatherLoadAsyncHandler LoadAsyncCompleted;
        private WebClient _wc;

        public static string ApiKey { get; set; }
        public string GetUrl(WeatherArea area, int days = 3)
        {
            // invalid api key
            // go get one from http://www.worldweatheronline.com/register.aspx
            if (string.IsNullOrEmpty(ApiKey))
                throw new InvalidOperationException("ApiKey is invalid.");

            // return the formatted url
            return string.Format(CultureInfo.InvariantCulture, _urlformat, ApiKey, area.Latitude, area.Longitude, days);
        }

        public WeatherData Load(WeatherArea area)
        {
            // get url string for this area
            string url = GetUrl(area);

            // new sync handle for wait operation
            using (EventWaitHandle wait = new ManualResetEvent(false))
            {
                // event complete args
                WeatherLoadAsyncEventArgs args = new WeatherLoadAsyncEventArgs() { WaitHandle = wait };

                WebClient wc = new WebClient();
                wc.OpenReadCompleted += new OpenReadCompletedEventHandler(WeatherLoader_LoadCompleted);
                wc.OpenReadAsync(new Uri(url, UriKind.Absolute), args);

                // wait for the call to complete
                wait.WaitOne();

                // return
                wait.Close();
                return args.Weather;
            }
        }

        private void WeatherLoader_LoadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            // restore state
            WeatherLoadAsyncEventArgs args = (WeatherLoadAsyncEventArgs)e.UserState;

            // call succeeded
            if (null == e.Error)
            {
                // deserialize weather information
                XmlReader reader = XmlReader.Create(e.Result);
                args.Weather = (WeatherData)_serializer.Deserialize(reader);
                reader.Close();

                args.WaitHandle.Set();
                return;
            }

            // call failed
            args.Error = e.Error;
            args.WaitHandle.Set();
        }

        public void LoadAsync(WeatherArea area)
        {
            LoadAsync(area, null);
        }

        public void LoadAsync(WeatherArea area, object userState)
        {
            // operation pending
            if (null != _wc)
                throw new InvalidOperationException("LoadAsync operation pending.");

            // get url string for this area
            string url = GetUrl(area);

            // event complete args
            WeatherLoadAsyncEventArgs args = new WeatherLoadAsyncEventArgs() { UserState = userState };

            // make the async call
            _wc = new WebClient();
            _wc.OpenReadCompleted += new OpenReadCompletedEventHandler(WeatherLoader_LoadAsyncCompleted);
            _wc.OpenReadAsync(new Uri(url, UriKind.Absolute), args);
        }

        public void CancelAsync()
        {
            _wc.CancelAsync();
            _wc = null;
        }

        private void WeatherLoader_LoadAsyncCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            // restore state
            WeatherLoadAsyncEventArgs args = (WeatherLoadAsyncEventArgs)e.UserState;

            // call succeeded
            if (null == e.Error)
            {
                // deserialize weather information
                XmlReader reader = XmlReader.Create(e.Result);
                args.Weather = (WeatherData)_serializer.Deserialize(reader);
                reader.Close();
            }

            // pass exception data
            args.Error = e.Error;

            // complete the async call
            this._wc = null;

            // notify callers
            if (null != this.LoadAsyncCompleted)
                this.LoadAsyncCompleted(this, args);
        }
    }
}
