using System.Collections.Generic;
using Phone.Controls.Samples;
using Weather;
using System.Windows.Media;

namespace WeatherForecast
{
    public class WeatherDataCollection : PivotItemCollectionAdaptor<WeatherData>
    {
        public WeatherDataCollection()
        {
        }

        public WeatherDataCollection(IList<WeatherData> list)
            : base(list)
        {
        }

        protected override WeatherData OnGetItem(PivotItem pivot)
        {
            WeatherControl ctrl = (WeatherControl)pivot.Content;
            return ctrl.Source;
        }

        protected override void OnSetItem(PivotItem pivot, WeatherData item)
        {
            WeatherControl ctrl = (WeatherControl)pivot.Content;
            ctrl.Source = item;
        }

        protected override void OnCreateItem(PivotItem pivot, WeatherData item)
        {
            pivot.Title = item.Area.Country.ToUpper();
            pivot.Header = item.Area.City.ToLower();
            pivot.Content = new WeatherControl()
            {
                Source = item
            };
        }
    }
}
