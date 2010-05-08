using System;
using System.Net;
using System.Threading;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;

namespace Weather
{
    public class WebLoader : IWeatherLoader
    {
        private const string _urlformat = "http://www.worldweatheronline.com/feed/weather.ashx?key={0}&lat={1}&lon={2}&num_of_days=5&includeLocation=yes&format=xml";
        private static XmlSerializer _serializer = new XmlSerializer(typeof(WeatherData));

        public event WeatherLoadAsyncHandler LoadAsyncCompleted;

        public static string ApiKey { get; set; }
        public string GetUrl(WeatherArea area)
        {
            // invalid api key
            // go get one from http://www.worldweatheronline.com/register.aspx
            if (string.IsNullOrEmpty(ApiKey))
                throw new InvalidOperationException("ApiKey is invalid.");

            // return the formatted url
            return string.Format(CultureInfo.InvariantCulture, _urlformat, ApiKey, area.Latitude, area.Longitude);
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

        private static void WeatherLoader_LoadCompleted(object sender, OpenReadCompletedEventArgs e)
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
            // get url string for this area
            string url = GetUrl(area);

            // event complete args
            WeatherLoadAsyncEventArgs args = new WeatherLoadAsyncEventArgs();

            // make the async call
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(WeatherLoader_LoadAsyncCompleted);
            wc.OpenReadAsync(new Uri(url, UriKind.Absolute), args);
        }

        private static void WeatherLoader_LoadAsyncCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            // restore state
            WebLoader loader = (WebLoader)sender;
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

            // notify callers
            if (null != loader.LoadAsyncCompleted)
                loader.LoadAsyncCompleted(loader, args);
        }
    }
}
