﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml;

namespace Weather
{
    public class WeatherStorage
    {
        #region Sample data
        public static readonly List<WeatherData> SampleData = new List<WeatherData>()
        {
            new WeatherData()
            {
                Area = new WeatherArea()
                {
                    City = WeatherData.LocalHeader,
                    Region = string.Empty,
                    Country = WeatherData.LocalTitle,
                    Longitude = double.NaN,
                    Latitude = double.NaN,
                },
                Detail = new WeatherDetail()
                {
                    Condition = Condition.All[0],
                },
            },
            new WeatherData()
            {
	            Area = new WeatherArea() { City = "Paris", Country = "France", Region = "Ile-de-France", Latitude = 48.87, Longitude = 2.33,},
	            Detail = new WeatherDetail() { Condition = Condition.All[353], Time = "08:16 PM", Date = DateTime.Parse("0001-01-01 00:00:00Z"), Temperature = 13, TemperatureMax = 0, TemperatureMin = 0, Precipitation = 0.2, CloudCover = 25, Humidity = 77, Visibility = 10, Pressure = 1009, WindSpeed = 6, WindDirection = "E",},
	            Forecast = new List<WeatherDetail>()
	            {
		            new WeatherDetail() { Condition = Condition.All[116], Time = "", Date = DateTime.Parse("2010-05-08 00:00:00Z"), Temperature = 0, TemperatureMax = 18, TemperatureMin = 9, Precipitation = 0.8, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 13, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[116], Time = "", Date = DateTime.Parse("2010-05-09 00:00:00Z"), Temperature = 0, TemperatureMax = 18, TemperatureMin = 9, Precipitation = 1, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 17, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[296], Time = "", Date = DateTime.Parse("2010-05-10 00:00:00Z"), Temperature = 0, TemperatureMax = 13, TemperatureMin = 9, Precipitation = 6.2, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 17, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[296], Time = "", Date = DateTime.Parse("2010-05-11 00:00:00Z"), Temperature = 0, TemperatureMax = 10, TemperatureMin = 6, Precipitation = 4.3, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 19, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[119], Time = "", Date = DateTime.Parse("2010-05-12 00:00:00Z"), Temperature = 0, TemperatureMax = 13, TemperatureMin = 5, Precipitation = 1, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 13, WindDirection = "",},
	            },
            },
            new WeatherData()
            {
	            Area = new WeatherArea() { City = "London", Country = "United Kingdom", Region = "", Latitude = 51.5, Longitude = -0.12,},
	            Detail = new WeatherDetail() { Condition = Condition.All[266], Time = "08:16 PM", Date = DateTime.Parse("0001-01-01 00:00:00Z"), Temperature = 8, TemperatureMax = 0, TemperatureMin = 0, Precipitation = 0.5, CloudCover = 50, Humidity = 81, Visibility = 10, Pressure = 1013, WindSpeed = 22, WindDirection = "NE",},
	            Forecast = new List<WeatherDetail>()
	            {
		            new WeatherDetail() { Condition = Condition.All[296], Time = "", Date = DateTime.Parse("2010-05-08 00:00:00Z"), Temperature = 0, TemperatureMax = 8, TemperatureMin = 6, Precipitation = 4.6, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 17, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[266], Time = "", Date = DateTime.Parse("2010-05-09 00:00:00Z"), Temperature = 0, TemperatureMax = 11, TemperatureMin = 6, Precipitation = 2.3, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 17, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[113], Time = "", Date = DateTime.Parse("2010-05-10 00:00:00Z"), Temperature = 0, TemperatureMax = 13, TemperatureMin = 3, Precipitation = 0, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 21, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[119], Time = "", Date = DateTime.Parse("2010-05-11 00:00:00Z"), Temperature = 0, TemperatureMax = 10, TemperatureMin = 3, Precipitation = 0.2, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 22, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[122], Time = "", Date = DateTime.Parse("2010-05-12 00:00:00Z"), Temperature = 0, TemperatureMax = 9, TemperatureMin = 3, Precipitation = 0.1, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 19, WindDirection = "",},
	            },
            },
            new WeatherData()
            {
	            Area = new WeatherArea() { City = "New York", Country = "United States of America", Region = "New York", Latitude = 40.71, Longitude = -74.01,},
	            Detail = new WeatherDetail() { Condition = Condition.All[113], Time = "08:16 PM", Date = DateTime.Parse("0001-01-01 00:00:00Z"), Temperature = 24, TemperatureMax = 0, TemperatureMin = 0, Precipitation = 0, CloudCover = 25, Humidity = 39, Visibility = 16, Pressure = 998, WindSpeed = 20, WindDirection = "W",},
	            Forecast = new List<WeatherDetail>()
	            {
		            new WeatherDetail() { Condition = Condition.All[176], Time = "", Date = DateTime.Parse("2010-05-08 00:00:00Z"), Temperature = 0, TemperatureMax = 22, TemperatureMin = 8, Precipitation = 2.5, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 36, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[116], Time = "", Date = DateTime.Parse("2010-05-09 00:00:00Z"), Temperature = 0, TemperatureMax = 12, TemperatureMin = 4, Precipitation = 0, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 29, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[113], Time = "", Date = DateTime.Parse("2010-05-10 00:00:00Z"), Temperature = 0, TemperatureMax = 14, TemperatureMin = 3, Precipitation = 0, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 22, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[113], Time = "", Date = DateTime.Parse("2010-05-11 00:00:00Z"), Temperature = 0, TemperatureMax = 15, TemperatureMin = 6, Precipitation = 9.7, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 17, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[266], Time = "", Date = DateTime.Parse("2010-05-12 00:00:00Z"), Temperature = 0, TemperatureMax = 18, TemperatureMin = 7, Precipitation = 19.7, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 21, WindDirection = "",},
	            },
            },
            new WeatherData()
            {
	            Area = new WeatherArea() { City = "Seattle", Country = "United States of America", Region = "Washington", Latitude = 47.61, Longitude = -122.33,},
	            Detail = new WeatherDetail() { Condition = Condition.All[113], Time = "08:16 PM", Date = DateTime.Parse("0001-01-01 00:00:00Z"), Temperature = 16, TemperatureMax = 0, TemperatureMin = 0, Precipitation = 0, CloudCover = 0, Humidity = 46, Visibility = 16, Pressure = 1018, WindSpeed = 0, WindDirection = "NNW",},
	            Forecast = new List<WeatherDetail>()
	            {
		            new WeatherDetail() { Condition = Condition.All[113], Time = "", Date = DateTime.Parse("2010-05-08 00:00:00Z"), Temperature = 0, TemperatureMax = 21, TemperatureMin = 4, Precipitation = 0, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 6, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[113], Time = "", Date = DateTime.Parse("2010-05-09 00:00:00Z"), Temperature = 0, TemperatureMax = 22, TemperatureMin = 4, Precipitation = 0, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 9, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[263], Time = "", Date = DateTime.Parse("2010-05-10 00:00:00Z"), Temperature = 0, TemperatureMax = 14, TemperatureMin = 6, Precipitation = 1.9, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 8, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[113], Time = "", Date = DateTime.Parse("2010-05-11 00:00:00Z"), Temperature = 0, TemperatureMax = 20, TemperatureMin = 5, Precipitation = 0, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 6, WindDirection = "",},
		            new WeatherDetail() { Condition = Condition.All[113], Time = "", Date = DateTime.Parse("2010-05-12 00:00:00Z"), Temperature = 0, TemperatureMax = 22, TemperatureMin = 5, Precipitation = 0, CloudCover = 0, Humidity = 0, Visibility = 0, Pressure = 0, WindSpeed = 11, WindDirection = "",},
	            },
            },
        };
        #endregion

        private const string _url = @"\cache.xml";
        private static XmlSerializer _serializer = new XmlSerializer(typeof(List<WeatherData>));

        public static IList<WeatherData> Load()
        {
            // return value
            List<WeatherData> weather = null;

            // deserialize weather information
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                using (Stream stream = store.OpenFile(_url, FileMode.Open))
                {
                    XmlReader reader = XmlReader.Create(stream);
                    weather = (List<WeatherData>)_serializer.Deserialize(reader);
                    reader.Close();
                }
            }
            catch
            {
                // failed reading cached file
                // assuming this is first launch : return fake data
                weather = SampleData;
            }

            // return weather info
            return weather;
        }

        public static void Save(IList<WeatherData> weather)
        {
            IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();

            // create a new file (overwrite existing)
            using (IsolatedStorageFileStream file = store.CreateFile(_url))
            {
                _serializer.Serialize(file, weather);
            }
        }
    }
}