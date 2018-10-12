using KzsRest.Engine.TypeConverters;
using System.ComponentModel;

namespace KzsRest.Engine
{
    [TypeConverter(typeof(DivisionTypeConverter))]
    public enum DivisionType
    {
        First,
        FirstA,
        FirstB,
        Second,
        SecondA,
        SecondB,
        FirstQualify
    }
}
