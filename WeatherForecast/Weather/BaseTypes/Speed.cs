using System;
using System.Globalization;

namespace Weather
{
    public class SpeedFormatInfo : IFormatProvider, ICustomFormatter
    {
        internal delegate double ConvertToMethod(Speed val);
        internal delegate Speed ConvertFromMethod(double val);

        public string Pattern { get; private set; }
        public string Suffix { get; private set; }
        internal ConvertToMethod ConvertTo { get; private set; }
        internal ConvertFromMethod ConvertFrom { get; private set; }

        private const string KilometersPerHourPattern = "K";
        private const string KilometersPerHourSuffix = " Kph";
        private const string MilesPerHourPattern = "M";
        private const string MilesPerHourSuffix = " mph";
        private const string KnotsPattern = "T";
        private const string KnotsSuffix = " Kt";
        private const string MetersPerSecondPattern = "S";
        private const string MetersPerSecondSuffix = " m/s";

        public static readonly SpeedFormatInfo KilometersPerHourInfo = new SpeedFormatInfo()
        {
            Pattern = KilometersPerHourPattern,
            Suffix = KilometersPerHourSuffix,
            ConvertTo = Speed.ToKilometersPerHour,
            ConvertFrom = Speed.FromKilometersPerHour
        };

        public static readonly SpeedFormatInfo MilesPerHourInfo = new SpeedFormatInfo()
        {
            Pattern = MilesPerHourPattern,
            Suffix = MilesPerHourSuffix,
            ConvertTo = Speed.ToMilesPerHour,
            ConvertFrom = Speed.FromMilesPerHour
        };

        public static readonly SpeedFormatInfo KnotsInfo = new SpeedFormatInfo()
        {
            Pattern = KnotsPattern,
            Suffix = KnotsSuffix,
            ConvertTo = Speed.ToKnots,
            ConvertFrom = Speed.FromKnots
        };

        public static readonly SpeedFormatInfo MetersPerSecondInfo = new SpeedFormatInfo()
        {
            Pattern = MetersPerSecondPattern,
            Suffix = MetersPerSecondSuffix,
            ConvertTo = Speed.ToMetersPerSecond,
            ConvertFrom = Speed.FromMetersPerSecond
        };

        public static readonly SpeedFormatInfo[] All = new SpeedFormatInfo[] {
            KilometersPerHourInfo,
            MilesPerHourInfo,
            KnotsInfo,
            MetersPerSecondInfo
        };

        public static SpeedFormatInfo CurrentInfo = All[0];

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
            Speed speed;

            // try coerce 'arg' to Speed
            if (arg.GetType() != typeof(Speed))
            {
                try
                {
                    speed = Convert.ToDouble(arg);
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format("The format of '{0}' is invalid.", format), e);
                }
            }
            else
            {
                speed = (Speed)arg;
            }

            // default string format
            if (string.IsNullOrEmpty(format))
            {
                format = CurrentInfo.Pattern;
            }

            // format to string
            string uformat = format.ToUpper(CultureInfo.InvariantCulture);
            foreach (var info in SpeedFormatInfo.All)
            {
                if (uformat == info.Pattern)
                    return info.ConvertTo(speed).ToString("0.#", formatProvider) + info.Suffix;
            }

            throw new FormatException(string.Format("The format of '{0}' is invalid.", format));
        }
        #endregion
    }

    public struct Speed : IComparable, IFormattable
    {
        private double _speed;

        public Speed(double val)
        {
            _speed = val;
        }

        public double KilometersPerHour
        {
            get { return ToKilometersPerHour(this); }
        }

        public double MilesPerHour
        {
            get { return ToMilesPerHour(this); }
        }

        public double Knots
        {
            get { return ToKnots(this); }
        }

        public double MetersPerSecond
        {
            get { return ToMetersPerSecond(this); }
        }

        #region Conversions
        public static implicit operator double(Speed val)
        {
            return val._speed;
        }

        public static implicit operator Speed(double val)
        {
            return new Speed(val);
        }

        public static double ToKilometersPerHour(Speed val)
        {
            return val._speed;
        }

        public static Speed FromKilometersPerHour(double val)
        {
            return new Speed(val);
        }

        public static double ToMilesPerHour(Speed val)
        {
            return val._speed * 0.621371192d;
        }

        public static Speed FromMilesPerHour(double val)
        {
            return new Speed(val * 1.609344d);
        }

        public static double ToKnots(Speed val)
        {
            return val._speed / 1.852d;
        }

        public static Speed FromKnots(double val)
        {
            return new Speed(val * 1.852d);
        }

        public static double ToMetersPerSecond(Speed val)
        {
            return val._speed / 3.6d;
        }

        public static Speed FromMetersPerSecond(double val)
        {
            return new Speed(val * 3.6d);
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            return _speed.CompareTo(obj);
        }
        #endregion

        #region Object
        public override bool Equals(object obj)
        {
            return _speed.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _speed.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(SpeedFormatInfo.CurrentInfo.Pattern, NumberFormatInfo.CurrentInfo);
        }
        #endregion

        #region IFormattable
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return SpeedFormatInfo.CurrentInfo.Format(format, this, formatProvider);
        }
        #endregion

        #region Parse
        public static Speed Parse(string value)
        {
            return Parse(value, CultureInfo.CurrentCulture);
        }

        public static Speed Parse(string value, IFormatProvider provider)
        {
            Speed result;
            if (!TryParse(value, provider, out result))
                throw new FormatException(string.Format("The format of '{0}' is invalid.", value));
            return result;
        }

        public static bool TryParse(string value, out Speed result)
        {
            return TryParse(value, CultureInfo.CurrentCulture, out result);
        }

        public static bool TryParse(string value, IFormatProvider provider, out Speed result)
        {
            double speed;

            // parse all suffixes
            foreach (SpeedFormatInfo info in SpeedFormatInfo.All)
            {
                if (value.EndsWith(info.Suffix, StringComparison.InvariantCultureIgnoreCase))
                {
                    int lenght = value.Length - info.Suffix.TrimStart().Length;
                    if (Double.TryParse(value.Substring(0, lenght), NumberStyles.Float, provider, out speed))
                    {
                        result = info.ConvertFrom(speed);
                        return true;
                    }
                }
            }

            // no suffix match, parse as double
            // and convert relative to CurrentInfo
            if (Double.TryParse(value, NumberStyles.Float, provider, out speed))
            {
                foreach (SpeedFormatInfo info in SpeedFormatInfo.All)
                {
                    if (SpeedFormatInfo.CurrentInfo == info)
                    {
                        result = info.ConvertFrom(speed);
                        return true;
                    }
                }
            }

            result = new Speed();
            return false;
        }
        #endregion
    }
}
