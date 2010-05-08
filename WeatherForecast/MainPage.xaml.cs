using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Phone.Controls.Samples;
using Weather;

namespace WeatherForecast
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            // get an ApiKey from http://www.worldweatheronline.com/register.aspx
            WebLoader.ApiKey = "0000000000000000000000";

            // for now, we'll just load fake data...
            var Paris = new WeatherArea() { Latitude = 48.870, Longitude = 2.330 };
            var London = new WeatherArea() { Latitude = 51.500, Longitude = -0.120 };
            var NewYork = new WeatherArea() { Latitude = 40.710, Longitude = -74.010 };
            var Seattle = new WeatherArea() { Latitude = 47.610, Longitude = -122.330 };
            LoadWeather(Paris);
            LoadWeather(London);
            LoadWeather(NewYork);
            LoadWeather(Seattle);
        }

        private void LoadWeather(WeatherArea area)
        {
            // make the async call
            ThreadPool.QueueUserWorkItem((state) =>
            {
                IWeatherLoader loader = new FakeLoader();
                WeatherData weather = loader.Load(area);

                // add weather data to new pivot tab
                pivot1.Dispatcher.BeginInvoke(() =>
                {
                    pivot1.Items.Add(new PivotItem()
                    {
                        Title = weather.Area.Country.ToUpper(),
                        Header = weather.Area.City.ToLower(),
                        Content = new WeatherControl()
                        {
                            Source = weather
                        }
                    });
                });
            });
        }
    }
}