using System;
using System.ComponentModel;
using System.Globalization;

namespace KzsRest.Models.TypeConverters
{
    public class DivisionTypeConverter: TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                if (string.Equals((string)value, "1", StringComparison.OrdinalIgnoreCase))
                {
                    return DivisionType.First;
                }
                else if (string.Equals((string)value, "1a", StringComparison.OrdinalIgnoreCase))
                {
                    return DivisionType.FirstA;
                }
                else if (string.Equals((string)value, "1b", StringComparison.OrdinalIgnoreCase))
                {
                    return DivisionType.FirstB;
                }
                else if (string.Equals((string)value, "2", StringComparison.OrdinalIgnoreCase))
                {
                    return DivisionType.Second;
                }
                else if (string.Equals((string)value, "2a", StringComparison.OrdinalIgnoreCase))
                {
                    return DivisionType.SecondA;
                }
                else if (string.Equals((string)value, "2b", StringComparison.OrdinalIgnoreCase))
                {
                    return DivisionType.SecondB;
                }
                else if (string.Equals((string)value, "1q", StringComparison.OrdinalIgnoreCase))
                {
                    return DivisionType.FirstQualify;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var division = (DivisionType)value;
                switch (division)
                {
                    case DivisionType.First: return "1";
                    case DivisionType.FirstA: return "1a";
                    case DivisionType.FirstB: return "1b";
                    case DivisionType.Second: return "2";
                    case DivisionType.SecondA: return "2a";
                    case DivisionType.SecondB: return "2b";
                    case DivisionType.FirstQualify: return "1q";
                    default: throw new Exception($"Unknown DivisionType {value}");
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
