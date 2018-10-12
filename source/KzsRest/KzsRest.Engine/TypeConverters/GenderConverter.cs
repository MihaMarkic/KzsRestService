using System;
using System.ComponentModel;
using System.Globalization;

namespace KzsRest.Engine.TypeConverters
{
    public class GenderConverter: TypeConverter
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
                if (string.Equals((string)value, "fantje", StringComparison.OrdinalIgnoreCase))
                {
                    return Gender.Male;
                }
                else if (string.Equals((string)value, "punce", StringComparison.OrdinalIgnoreCase))
                {
                    return Gender.Female;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
