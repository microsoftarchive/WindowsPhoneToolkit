using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Device.Location;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using Phone.Controls.Samples;
using Weather;
using Microsoft.Phone.Shell;

namespace WeatherForecast
{
    public partial class MainPage : PhoneApplicationPage
    {
        // get an ApiKey from http://www.worldweatheronline.com/register.aspx
        //private const string ApiKey = "0000000000000000000000";
        private const string ApiKey = "749e16bb43153527100205";

        public static WeatherDataCollection WeatherItems;

        #region Initialize
        public MainPage()
        {
            AreaSearchService.ApiKey = ApiKey;
            WeatherService.ApiKey = ApiKey;

            InitializeComponent();

            // load weather data from storage and insert local
            WeatherItems = new WeatherDataCollection(WeatherStorage.Load());
            pivot1.SelectionChanged += new SelectionChangedEventHandler(SelectionChanged);
            pivot1.ItemsSource = WeatherItems;

            // manage application lifecycle
            PhoneApplicationService.Current.Deactivated += new EventHandler<DeactivatedEventArgs>(PhoneApplicationService_Deactivated);
            PhoneApplicationService.Current.Closing += new EventHandler<ClosingEventArgs>(PhoneApplicationService_Closing);

            // start geo watcher
            StartLocationService(GeoPositionAccuracy.Default);
        }
        #endregion

        #region GeoCoordinateWatcher
        private GeoCoordinateWatcher _geo_watcher;
        private void StartLocationService(GeoPositionAccuracy accuracy)
        {
#if DEBUG
            // no location support in the emulator
            // simulate a GPS fix for Paris, FR
            WeatherItems[0].Area.Latitude = 48.844908714294434;
            WeatherItems[0].Area.Longitude = 2.2844648361206055;
            return;
#endif

            // reinitialize the GeoCoordinateWatcher
            _geo_watcher = new GeoCoordinateWatcher(accuracy);
            _geo_watcher.MovementThreshold = 20;

            // add event handlers for StatusChanged and PositionChanged events
            _geo_watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(PositionChanged);

            // start data acquisition
            _geo_watcher.Start();
        }

        private void PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            GeoCoordinateWatcher watcher = (GeoCoordinateWatcher)sender;
            if (watcher.Status == GeoPositionStatus.Ready)
            {
                WeatherItems[0].Area.Latitude = e.Position.Location.Latitude;
                WeatherItems[0].Area.Longitude = e.Position.Location.Longitude;
            }
        }
        #endregion

        #region Refresh
        private void Refresh(PivotItem item)
        {
            WeatherData weather = WeatherItems[item];

            // skip (0,0) location
            if (!double.IsNaN(weather.Area.Longitude) &&
                !double.IsNaN(weather.Area.Latitude))
            {
                WeatherService ws = new WeatherService();
                ws.LoadAsyncCompleted += new WeatherLoadAsyncHandler(RefreshAsyncCompleted);
                ws.LoadAsync(weather.Area, item);
            }
        }

        void RefreshAsyncCompleted(object sender, WeatherLoadAsyncEventArgs e)
        {
            if (null == e.Error)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    PivotItem item = (PivotItem)e.UserState;
                    WeatherData weather = WeatherItems[item];
                    e.Weather.Area = weather.Area;
                    WeatherItems[item] = e.Weather;
                });
            }
        }
        #endregion

        #region AppBar
        private void AreaRefresh(object sender, EventArgs e)
        {
            foreach (PivotItem item in pivot1.Items)
            {
                Refresh(item);
            }
        }

        private void AreaAdd(object sender, EventArgs e)
        {
            Uri uri = new Uri("/AreaSearch.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void AreaDelete(object sender, EventArgs e)
        {
            // selected item
            PivotItem item = pivot1.SelectedItem;
            if (null != item)
            {
                // selected weather data
                WeatherData weather = WeatherItems[item];

                // delete selected area
                string szTitle = weather.Area.City;
                string szMessage = "Are you sure you want to delete this Area ?";

                if (MessageBox.Show(szMessage, szTitle, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    WeatherItems.Remove(weather);
                }
            }
        }

        void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            const int iCmdDel = 2;

            if (null != ApplicationBar)
            {
                ApplicationBarIconButton button = (ApplicationBarIconButton)ApplicationBar.Buttons[iCmdDel];
                button.IsEnabled = (pivot1.SelectedIndex != 0);
            }
        }
        #endregion

        #region Lifecycle
        void PhoneApplicationService_Deactivated(object sender, DeactivatedEventArgs e)
        {
            WeatherStorage.Save(WeatherItems.Items);
        }

        void PhoneApplicationService_Closing(object sender, ClosingEventArgs e)
        {
            WeatherStorage.Save(WeatherItems.Items);
        }
        #endregion
    }
}