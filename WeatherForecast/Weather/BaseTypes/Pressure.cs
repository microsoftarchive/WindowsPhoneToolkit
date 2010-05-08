using System;
using System.Globalization;

namespace Weather
{
    public class PresureFormatInfo : IFormatProvider, ICustomFormatter
    {
        internal delegate double ConvertToMethod(Pressure val);
        internal delegate Pressure ConvertFromMethod(double val);

        public string Pattern { get; private set; }
        public string Suffix { get; private set; }
        internal ConvertToMethod ConvertTo { get; private set; }
        internal ConvertFromMethod ConvertFrom { get; private set; }

        private const string MillibarsPattern = "M";
        private const string MillibarsSuffix = " mm";
        private const string InchesPattern = "I";
        private const string InchesSuffix = " In";

        public static readonly PresureFormatInfo MillibarsInfo = new PresureFormatInfo()
        {
            Pattern = MillibarsPattern,
            Suffix = MillibarsSuffix,
            ConvertTo = Pressure.ToMillibars,
            ConvertFrom = Pressure.FromMillibars
        };

        public static readonly PresureFormatInfo InchesInfo = new PresureFormatInfo()
        {
            Pattern = InchesPattern,
            Suffix = InchesSuffix,
            ConvertTo = Pressure.ToInches,
            ConvertFrom = Pressure.FromInches
        };

        public static readonly PresureFormatInfo[] All = new PresureFormatInfo[] {
            MillibarsInfo,
            InchesInfo
        };

        public static PresureFormatInfo CurrentInfo = All[0];

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
            Pressure pressure;

            // try coerce 'arg' to Pressure
            if (arg.GetType() != typeof(Pressure))
            {
                try
                {
                    pressure = Convert.ToDouble(arg);
                }
                catch (Exception e)
                {
                    throw new FormatException(string.Format("The format of '{0}' is invalid.", format), e);
                }
            }
            else
            {
                pressure = (Pressure)arg;
            }

            // default string format
            if (string.IsNullOrEmpty(format))
            {
                format = CurrentInfo.Pattern;
            }

            // format to string
            string uformat = format.ToUpper(CultureInfo.InvariantCulture);
            foreach (var info in PresureFormatInfo.All)
            {
                if (uformat == info.Pattern)
                    return info.ConvertTo(pressure).ToString("0.#", formatProvider) + info.Suffix;
            }

            throw new FormatException(string.Format("The format of '{0}' is invalid.", format));
        }
        #endregion
    }

    public struct Pressure : IComparable, IFormattable
    {
        private double _pressure;

        public Pressure(double val)
        {
            _pressure = val;
        }

        public double Millibars
        {
            get { return ToMillibars(this); }
        }

        public double Inches
        {
            get { return ToInches(this); }
        }

        #region Conversions
        public static implicit operator double(Pressure val)
        {
            return val._pressure;
        }

        public static implicit operator Pressure(double val)
        {
            return new Pressure(val);
        }

        public static double ToMillibars(Pressure val)
        {
            return val._pressure;
        }

        public static Pressure FromMillibars(double val)
        {
            return new Pressure(val);
        }

        public static double ToInches(Pressure val)
        {
            return val._pressure * 0.0295301d;
        }

        public static Pressure FromInches(double val)
        {
            return new Pressure(val * 33.8637526d);
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            return _pressure.CompareTo(obj);
        }
        #endregion

        #region Object
        public override bool Equals(object obj)
        {
            return _pressure.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _pressure.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(PresureFormatInfo.CurrentInfo.Pattern, NumberFormatInfo.CurrentInfo);
        }
        #endregion

        #region IFormattable
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return PresureFormatInfo.CurrentInfo.Format(format, this, formatProvider);
        }
        #endregion

        #region Parse
        public static Pressure Parse(string value)
        {
            return Parse(value, CultureInfo.CurrentCulture);
        }

        public static Pressure Parse(string value, IFormatProvider provider)
        {
            Pressure result;
            if (!TryParse(value, provider, out result))
                throw new FormatException(string.Format("The format of '{0}' is invalid.", value));
            return result;
        }

        public static bool TryParse(string value, out Pressure result)
        {
            return TryParse(value, CultureInfo.CurrentCulture, out result);
        }

        public static bool TryParse(string value, IFormatProvider provider, out Pressure result)
        {
            double pressure;

            // parse all suffixes
            foreach (PresureFormatInfo info in PresureFormatInfo.All)
            {
                if (value.EndsWith(info.Suffix, StringComparison.InvariantCultureIgnoreCase))
                {
                    int lenght = value.Length - info.Suffix.TrimStart().Length;
                    if (Double.TryParse(value.Substring(0, lenght), NumberStyles.Float, provider, out pressure))
                    {
                        result = info.ConvertFrom(pressure);
                        return true;
                    }
                }
            }

            // no suffix match, parse as double
            // and convert relative to CurrentInfo
            if (Double.TryParse(value, NumberStyles.Float, provider, out pressure))
            {
                foreach (PresureFormatInfo info in PresureFormatInfo.All)
                {
                    if (PresureFormatInfo.CurrentInfo == info)
                    {
                        result = info.ConvertFrom(pressure);
                        return true;
                    }
                }
            }

            result = new Pressure();
            return false;
        }
        #endregion
    }
}
