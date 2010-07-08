using System;
using System.Windows.Controls;
using System.Windows;
using Weather;

namespace WeatherForecast
{
    public class WeatherControl : Control
    {
        public WeatherControl()
        {
            this.DefaultStyleKey = typeof(WeatherControl);
        }

        #region SampleData
        public static readonly DependencyProperty SampleDataProperty =
            DependencyProperty.Register(
                    "SampleData",
                    typeof(bool),
                    typeof(WeatherControl),
                    new PropertyMetadata(OnSampleDataChanged));

        public bool SampleData
        {
            get { return (bool)GetValue(SampleDataProperty); }
            set { SetValue(SampleDataProperty, value); }
        }

        public static void OnSampleDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WeatherControl ctrl = (WeatherControl)d;
            if ((bool)e.NewValue)
            {
                ctrl.Source = WeatherStorage.SampleData[1];
            }
        }
        #endregion

        #region Source
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                    "Source",
                    typeof(WeatherData),
                    typeof(WeatherControl),
                    new PropertyMetadata(OnSourceChanged));

        public WeatherData Source
        {
            get { return (WeatherData)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WeatherControl ctrl = (WeatherControl)d;
            ctrl.OnSourceChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnSourceChanged(object oldHeader, object newHeader)
        {
        }
        #endregion
    }
}
