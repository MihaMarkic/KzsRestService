using KzsRest.Models.TypeConverters;
using System.ComponentModel;

namespace KzsRest.Models
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
