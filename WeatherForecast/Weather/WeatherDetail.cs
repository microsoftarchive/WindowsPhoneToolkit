using System;
using System.Xml.Serialization;
using System.Globalization;

namespace Weather
{
    public class WeatherDetail
    {
        private int _code;

        [XmlIgnore]
        public Condition Condition
        {
            get { return Condition.All[_code]; }
            set
            {
                _code = value.Code;

                Description = value.Desc;
                IconDayUrl = value.IconDay;
                IconNightUrl = value.IconNight;
            }
        }

        [XmlIgnore]
        public string Description { get; private set; }

        [XmlIgnore]
        public string IconDayUrl { get; private set; }

        [XmlIgnore]
        public string IconNightUrl { get; private set; }

        [XmlIgnore]
        public string IconUrl
        {
            get
            {
                // no time information, assume day time
                if (!string.IsNullOrEmpty(Time))
                {
                    DateTime time;
                    if (DateTime.TryParse(Time, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out time))
                    {
                        // assume sun is up from 6 am to 6 pm
                        if ((time.Hour < 6) || (time.Hour > 18))
                            return IconNightUrl;
                    }
                }

                // assume day time
                return IconDayUrl;
            }
        }

        [XmlElement("observation_time")]
        public string Time { get; set; }

        [XmlElement("date")]
        public DateTime Date { get; set; }

        [XmlIgnore]
        public Temperature Temperature { get; set; }

        [XmlIgnore]
        public Temperature TemperatureMax { get; set; }

        [XmlIgnore]
        public Temperature TemperatureMin { get; set; }

        [XmlIgnore]
        public Precipitation Precipitation { get; set; }

        [XmlElement("cloudcover")]
        public int CloudCover { get; set; }

        [XmlElement("humidity")]
        public int Humidity { get; set; }

        [XmlElement("visibility")]
        public int Visibility { get; set; }

        [XmlIgnore]
        public Pressure Pressure { get; set; }

        [XmlIgnore]
        public Speed WindSpeed { get; set; }

        [XmlElement("winddir16Point")]
        public string WindDirection { get; set; }

        #region Accessors for custom types
        [XmlElement("weatherCode")]
        public int __xml_accessor_Condition
        {
            get { return Condition.Code; }
            set { Condition = Condition.All[value]; }
        }

        [XmlElement("temp_C")]
        public double __xml_accessor_Temperature
        {
            get { return Temperature; }
            set { Temperature = value; }
        }

        [XmlElement("tempMaxC")]
        public double __xml_accessor_TemperatureMax
        {
            get { return TemperatureMax; }
            set { TemperatureMax = value; }
        }

        [XmlElement("tempMinC")]
        public double __xml_accessor_TemperatureMin
        {
            get { return TemperatureMin; }
            set { TemperatureMin = value; }
        }

        [XmlElement("precipMM")]
        public double __xml_accessor_Precipitation
        {
            get { return Precipitation; }
            set { Precipitation = value; }
        }

        [XmlElement("pressure")]
        public double __xml_accessor_Pressure
        {
            get { return Pressure; }
            set { Pressure = value; }
        }

        [XmlElement("windspeedKmph")]
        public double __xml_accessor_WindSpeed
        {
            get { return WindSpeed; }
            set { WindSpeed = value; }
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{");
            sb.AppendFormat(" Condition = Condition.All[{0}],", Condition.Code);
            sb.AppendFormat(" Time = \"{0:u}\",", Time);
            sb.AppendFormat(" Date = DateTime.Parse(\"{0:u}\"),", Date);
            sb.AppendFormat(" Temperature = {0},", (double)Temperature);
            sb.AppendFormat(" TemperatureMax = {0},", (double)TemperatureMax);
            sb.AppendFormat(" TemperatureMin = {0},", (double)TemperatureMin);
            sb.AppendFormat(" Precipitation = {0},", (double)Precipitation);
            sb.AppendFormat(" CloudCover = {0},", CloudCover);
            sb.AppendFormat(" Humidity = {0},", Humidity);
            sb.AppendFormat(" Visibility = {0},", Visibility);
            sb.AppendFormat(" Pressure = {0},", (double)Pressure);
            sb.AppendFormat(" WindSpeed = {0},", (double)WindSpeed);
            sb.AppendFormat(" WindDirection = \"{0}\",", WindDirection);
            sb.Append("}");

            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            return sb.ToString();
        }
        #endregion
    }
}
