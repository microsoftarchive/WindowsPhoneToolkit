using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using Weather;

namespace WeatherForecast
{
    public partial class AreaSearch : PhoneApplicationPage
    {
        private List<WeatherArea> Areas;
        private string FormatListItem(WeatherArea area)
        {
            // rename country (US/UK)
            string szCountry = area.Country;
            if (szCountry.Equals("United States of America", StringComparison.InvariantCultureIgnoreCase))
                szCountry = "US";
            if (szCountry.Equals("United Kingdom", StringComparison.InvariantCultureIgnoreCase))
                szCountry = "UK";
            
            // no region
            if (string.IsNullOrEmpty(area.Region))
                return string.Format("{0} ({1})", area.City, szCountry);

            return string.Format("{0}, {2} ({1})", area.City, szCountry, area.Region);
        }

        public AreaSearch()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            AreaSearchService ws = new AreaSearchService();
            ws.SearchAsyncCompleted += new AreaSearchAsyncHandler(AreaSearch_SearchAsyncCompleted);
            ws.SearchAsync(txtCity.Text);
        }

        void AreaSearch_SearchAsyncCompleted(object sender, AreaSearchAsyncEventArgs e)
        {
            // clear the cities list and reset areas data
            lstAreas.Items.Clear();
            btnOk.IsEnabled = false;
            this.Areas = e.Areas;

            // error
            if (null == this.Areas) return;

            // populate cities list
            foreach (var area in this.Areas)
	        {
                lstAreas.Items.Add(FormatListItem(area));
	        }
        }

        private void lstAreas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOk.IsEnabled = true;
        }
        
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            WeatherArea area = this.Areas[lstAreas.SelectedIndex];
            WeatherService ws = new WeatherService();
            ws.LoadAsyncCompleted += new WeatherLoadAsyncHandler(WeatherService_LoadAsyncCompleted);
            ws.LoadAsync(area, area);
        }

        void WeatherService_LoadAsyncCompleted(object sender, WeatherLoadAsyncEventArgs e)
        {
            if (null == e.Error)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    e.Weather.Area = (WeatherArea)e.UserState;
                    MainPage.WeatherItems.Add(e.Weather);
                    NavigationService.GoBack();
                });
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}