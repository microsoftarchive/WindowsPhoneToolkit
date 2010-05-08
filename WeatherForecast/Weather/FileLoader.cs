using System;
using System.Net;
using System.Threading;
using System.Globalization;
using System.Xml.Serialization;
using System.Xml;

namespace Weather
{
    public class FileLoader : IWeatherLoader
    {
        private const string _urlformat = @"{0}\{1}{2}.xml";
        private static XmlSerializer _serializer = new XmlSerializer(typeof(WeatherData));

        public event WeatherLoadAsyncHandler LoadAsyncCompleted;

        public static string RootPath { get; set; }
        public string GetUrl(WeatherArea area)
        {
            // invalid root path
            if (string.IsNullOrEmpty(RootPath))
                throw new InvalidOperationException("ApiKey is invalid.");

            // return the formatted url
            string lat = area.Latitude.ToString("000.0000", CultureInfo.InvariantCulture);
            string lon = area.Longitude.ToString("000.0000", CultureInfo.InvariantCulture);
            if (area.Longitude >= 0) lon = "+" + lon;
            return string.Format(CultureInfo.InvariantCulture, _urlformat, RootPath, lat, lon);
        }

        public WeatherData Load(WeatherArea area)
        {
            // get url string for this area
            string url = GetUrl(area);

            // deserialize weather information
            XmlReader reader = XmlReader.Create(url);
            WeatherData weather = (WeatherData)_serializer.Deserialize(reader);
            reader.Close();

            return weather;
        }

        public void LoadAsync(WeatherArea area)
        {
            // event complete args
            WeatherLoadAsyncEventArgs args = new WeatherLoadAsyncEventArgs() { Weather = Load(area) };

            if (null != LoadAsyncCompleted)
                LoadAsyncCompleted(this, args);
        }
    }
}
