using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Weather
{
    public class WeatherArea
    {
        [XmlElement("areaName")]
        public string City { get; set; }

        [XmlElement("country")]
        public string Country { get; set; }

        [XmlElement("region")]
        public string Region { get; set; }

        [XmlElement("latitude")]
        public double Latitude { get; set; }

        [XmlElement("longitude")]
        public double Longitude { get; set; }

        #region ToString
        public override string ToString()
        {
            System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{");
            sb.AppendFormat(" City = \"{0}\",", City);
            sb.AppendFormat(" Country = \"{0}\",", Country);
            sb.AppendFormat(" Region = \"{0}\",", Region);
            sb.AppendFormat(" Latitude = {0},", Latitude);
            sb.AppendFormat(" Longitude = {0},", Longitude);
            sb.Append("}");

            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            return sb.ToString();
        }
        #endregion
    }
}
