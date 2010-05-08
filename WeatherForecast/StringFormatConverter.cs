using System;
using System.Windows.Data;
using System.Globalization;

namespace WeatherForecast
{
    public class StringFormatConverter : IValueConverter
    {
        // This converts the object to the string to display.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Retrieve the format string and use it to format the value.
            string formatString = parameter as string;
            if (!string.IsNullOrEmpty(formatString))
            {
                return string.Format(culture, formatString, value);
            }

            // If the format string is null or empty, simply call ToString()
            // on the value.
            return value.ToString();
        }

        // No need to implement converting back on a one-way binding 
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
