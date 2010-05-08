using System;
using System.Globalization;

namespace Weather
{
    public class PrecipitationFormatInfo : IFormatProvider, ICustomFormatter
    {
        internal delegate double ConvertToMethod(Precipitation val);
        internal delegate Precipitation ConvertFromMethod(double val);

        public string Pattern { get; private set; }
        public string Suffix { get; private set; }
        internal ConvertToMethod ConvertTo { get; private set; }
        internal ConvertFromMethod ConvertFrom { get; private set; }

        private const string MillimetersPattern = "M";
        private const string MillibarsSuffix = " mm";
        private const string InchesPattern = "I";
        private const string InchesSuffix = " In";

        public static readonly PrecipitationFormatInfo MillimetersInfo = new PrecipitationFormatInfo()
        {
            Pattern = MillimetersPattern,
            Suffix = MillibarsSuffix,
            ConvertTo = Precipitation.ToMillimeters,
            ConvertFrom = Precipitation.FromMillimeters
        };

        public static readonly PrecipitationFormatInfo InchesInfo = new PrecipitationFormatInfo()
        {
            Pattern = InchesPattern,
            Suffix = InchesSuffix,
            ConvertTo = Precipitation.ToInches,
            ConvertFrom = Precipitation.FromInches
        };

        public static readonly PrecipitationFormatInfo[] All = new PrecipitationFormatInfo[] {
            MillimetersInfo,
            InchesInfo
        };

        public static PrecipitationFormatInfo CurrentInfo = All[0];

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
            Precipitation precipitation;

            // try coerce 'arg' to Precipitation
            if (arg.GetType() != typeof(Precipitation))
            {
                try
                {
                    precipitation = Convert.ToDouble(arg);
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format("The format of '{0}' is invalid.", format), e);
                }
            }
            else
            {
                precipitation = (Precipitation)arg;
            }

            // default string format
            if (string.IsNullOrEmpty(format))
            {
                format = CurrentInfo.Pattern;
            }

            // format to string
            string uformat = format.ToUpper(CultureInfo.InvariantCulture);
            foreach (var info in PrecipitationFormatInfo.All)
            {
                if (uformat == info.Pattern)
                    return info.ConvertTo(precipitation).ToString("0.#", formatProvider) + info.Suffix;
            }

            throw new FormatException(string.Format("The format of '{0}' is invalid.", format));
        }
        #endregion
    }

    public struct Precipitation : IComparable, IFormattable
    {
        private double _precipitation;

        public Precipitation(double val)
        {
            _precipitation = val;
        }

        public double Millimeters
        {
            get { return ToMillimeters(this); }
        }

        public double Inches
        {
            get { return ToInches(this); }
        }

        #region Conversions
        public static implicit operator double(Precipitation val)
        {
            return val._precipitation;
        }

        public static implicit operator Precipitation(double val)
        {
            return new Precipitation(val);
        }

        public static double ToMillimeters(Precipitation val)
        {
            return val._precipitation;
        }

        public static Precipitation FromMillimeters(double val)
        {
            return new Precipitation(val);
        }

        public static double ToInches(Precipitation val)
        {
            return val._precipitation / 2.54d;
        }

        public static Precipitation FromInches(double val)
        {
            return new Precipitation(val * 2.54d);
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            return _precipitation.CompareTo(obj);
        }
        #endregion

        #region Object
        public override bool Equals(object obj)
        {
            return _precipitation.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _precipitation.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(PrecipitationFormatInfo.CurrentInfo.Pattern, NumberFormatInfo.CurrentInfo);
        }
        #endregion

        #region IFormattable
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return PrecipitationFormatInfo.CurrentInfo.Format(format, this, formatProvider);
        }
        #endregion

        #region Parse
        public static Precipitation Parse(string value)
        {
            return Parse(value, CultureInfo.CurrentCulture);
        }

        public static Precipitation Parse(string value, IFormatProvider provider)
        {
            Precipitation result;
            if (!TryParse(value, provider, out result))
                throw new FormatException(string.Format("The format of '{0}' is invalid.", value));
            return result;
        }

        public static bool TryParse(string value, out Precipitation result)
        {
            return TryParse(value, CultureInfo.CurrentCulture, out result);
        }

        public static bool TryParse(string value, IFormatProvider provider, out Precipitation result)
        {
            double precipitation;

            // parse all suffixes
            foreach (PrecipitationFormatInfo info in PrecipitationFormatInfo.All)
            {
                if (value.EndsWith(info.Suffix, StringComparison.InvariantCultureIgnoreCase))
                {
                    int lenght = value.Length - info.Suffix.TrimStart().Length;
                    if (Double.TryParse(value.Substring(0, lenght), NumberStyles.Float, provider, out precipitation))
                    {
                        result = info.ConvertFrom(precipitation);
                        return true;
                    }
                }
            }

            // no suffix match, parse as double
            // and convert relative to CurrentInfo
            if (Double.TryParse(value, NumberStyles.Float, provider, out precipitation))
            {
                foreach (PrecipitationFormatInfo info in PrecipitationFormatInfo.All)
                {
                    if (PrecipitationFormatInfo.CurrentInfo == info)
                    {
                        result = info.ConvertFrom(precipitation);
                        return true;
                    }
                }
            }

            result = new Precipitation();
            return false;
        }
        #endregion
    }
}
