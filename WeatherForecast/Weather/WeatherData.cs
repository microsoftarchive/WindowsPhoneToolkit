using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Weather
{
    [XmlRoot("data")]
    public class WeatherData
    {
        [XmlElement("nearest_area")]
        public WeatherArea Area { get; set; }

        [XmlElement("current_condition")]
        public WeatherDetail Detail { get; set; }

        [XmlElement("weather")]
        public List<WeatherDetail> Forecast { get; set; }

        public WeatherData()
        {
            Area = new WeatherArea();
            Detail = new WeatherDetail();
            Forecast = new List<WeatherDetail>();
        }

        #region ToString
        public override string ToString()
        {
            System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{\n");
            sb.AppendFormat("\tArea = new WeatherArea() {0},\n", Area);
            sb.AppendFormat("\tDetail = new WeatherDetail() {0},\n", Detail);
            sb.AppendFormat("\tForecast = new List<WeatherDetail>()\n");
            sb.Append("\t{\n");
            foreach (var data in Forecast)
            {
                sb.AppendFormat("\t\tnew WeatherDetail() {0},\n", data);
            }
            sb.Append("\t},\n");
            sb.Append("}");

            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            return sb.ToString();
        }
        #endregion
    }
}
