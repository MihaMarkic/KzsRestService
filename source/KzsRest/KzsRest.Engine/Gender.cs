using KzsRest.Engine.TypeConverters;
using System.ComponentModel;

namespace KzsRest.Engine
{
    [TypeConverter(typeof(GenderConverter))]
    public enum Gender
    {
        Male,
        Female
    }
}
