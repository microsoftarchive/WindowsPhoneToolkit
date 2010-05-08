using System;
using System.Globalization;

namespace Weather
{
    public class TemperatureFormatInfo : IFormatProvider, ICustomFormatter
    {
        internal delegate double ConvertToMethod(Temperature val);
        internal delegate Temperature ConvertFromMethod(double val);

        public string Pattern { get; private set; }
        public string Suffix { get; private set; }
        internal ConvertToMethod ConvertTo { get; private set; }
        internal ConvertFromMethod ConvertFrom { get; private set; }

        private const string CelsiusPattern = "C";
        private const string CelsiusSuffix = "°C";
        private const string KelvinPattern = "K";
        private const string KelvinSuffix = "K";
        private const string FahrenheitPattern = "F";
        private const string FahrenheitSuffix = "°F";

        public static readonly TemperatureFormatInfo CelsiusInfo = new TemperatureFormatInfo()
        {
            Pattern = CelsiusPattern,
            Suffix = CelsiusSuffix,
            ConvertTo = Temperature.ToCelsius,
            ConvertFrom = Temperature.FromCelsius
        };

        public static readonly TemperatureFormatInfo KelvinInfo = new TemperatureFormatInfo()
        {
            Pattern = KelvinPattern,
            Suffix = KelvinSuffix,
            ConvertTo = Temperature.ToKelvin,
            ConvertFrom = Temperature.FromKelvin
        };

        public static readonly TemperatureFormatInfo FahrenheitInfo = new TemperatureFormatInfo()
        {
            Pattern = FahrenheitPattern,
            Suffix = FahrenheitSuffix,
            ConvertTo = Temperature.ToFahrenheit,
            ConvertFrom = Temperature.FromFahrenheit
        };

        public static readonly TemperatureFormatInfo[] All = new TemperatureFormatInfo[] {
            CelsiusInfo,
            KelvinInfo,
            FahrenheitInfo
        };

        public static TemperatureFormatInfo CurrentInfo = All[0];

        #region IFormatProvider
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            return null;
        }
        #endregion

        #region ICustomFormatter
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            Type type = arg.GetType();
            Temperature temperature;

            // try coerce 'arg' to Temperature
            if (arg.GetType() != typeof(Temperature))
            {
                try
                {
                    temperature = Convert.ToDouble(arg);
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format("The format of '{0}' is invalid.", format), e);
                }
            }
            else
            {
                temperature = (Temperature)arg;
            }

            // default string format
            if (string.IsNullOrEmpty(format))
            {
                format = CurrentInfo.Pattern;
            }

            // format to string
            string uformat = format.ToUpper(CultureInfo.InvariantCulture);
            foreach (var info in TemperatureFormatInfo.All)
            {
                if (uformat == info.Pattern)
                    return info.ConvertTo(temperature).ToString("0.#", formatProvider) + info.Suffix;
            }

            throw new FormatException(string.Format("The format of '{0}' is invalid.", format));
        }
        #endregion
    }

    public struct Temperature : IComparable, IFormattable
    {
        private double _temperature;

        public Temperature(double val)
        {
            _temperature = val;
        }

        public double Celsius
        {
            get { return ToCelsius(this); }
        }

        public double Kelvin
        {
            get { return ToKelvin(this); }
        }

        public double Fahrenheit
        {
            get { return ToFahrenheit(this); }
        }

        #region Conversions
        public static implicit operator double(Temperature val)
        {
            return val._temperature;
        }

        public static implicit operator Temperature(double val)
        {
            return new Temperature(val);
        }

        public static double ToCelsius(Temperature val)
        {
            return val._temperature;
        }

        public static Temperature FromCelsius(double val)
        {
            return new Temperature(val);
        }

        public static double ToKelvin(Temperature val)
        {
            return val._temperature + 273.15d;
        }

        public static Temperature FromKelvin(double val)
        {
            return new Temperature(val - 273.15d);
        }

        public static double ToFahrenheit(Temperature val)
        {
            return 9d / 5d * val._temperature + 32d;
        }

        public static Temperature FromFahrenheit(double val)
        {
            return new Temperature(5d / 9d * (val - 32d));
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            return _temperature.CompareTo(obj);
        }
        #endregion

        #region Object
        public override bool Equals(object obj)
        {
            return _temperature.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _temperature.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(TemperatureFormatInfo.CurrentInfo.Pattern, NumberFormatInfo.CurrentInfo);
        }
        #endregion

        #region IFormattable
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return TemperatureFormatInfo.CurrentInfo.Format(format, this, formatProvider);
        }
        #endregion

        #region Parse
        public static Temperature Parse(string value)
        {
            return Parse(value, CultureInfo.CurrentCulture);
        }

        public static Temperature Parse(string value, IFormatProvider provider)
        {
            Temperature result;
            if (!TryParse(value, provider, out result))
                throw new FormatException(string.Format("The format of '{0}' is invalid.", value));
            return result;
        }

        public static bool TryParse(string value, out Temperature result)
        {
            return TryParse(value, CultureInfo.CurrentCulture, out result);
        }

        public static bool TryParse(string value, IFormatProvider provider, out Temperature result)
        {
            double temperature;

            // parse all suffixes
            foreach (TemperatureFormatInfo info in TemperatureFormatInfo.All)
            {
                if (value.EndsWith(info.Suffix, StringComparison.InvariantCultureIgnoreCase))
                {
                    int lenght = value.Length - info.Suffix.TrimStart().Length;
                    if (Double.TryParse(value.Substring(0, lenght), NumberStyles.Float, provider, out temperature))
                    {
                        result = info.ConvertFrom(temperature);
                        return true;
                    }
                }
            }

            // no suffix match, parse as double
            // and convert relative to CurrentInfo
            if (Double.TryParse(value, NumberStyles.Float, provider, out temperature))
            {
                foreach (TemperatureFormatInfo info in TemperatureFormatInfo.All)
                {
                    if (TemperatureFormatInfo.CurrentInfo == info)
                    {
                        result = info.ConvertFrom(temperature);
                        return true;
                    }
                }
            }

            result = new Temperature();
            return false;
        }
        #endregion
    }
}
