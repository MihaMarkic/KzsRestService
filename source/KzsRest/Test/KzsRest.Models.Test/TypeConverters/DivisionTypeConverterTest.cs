using KzsRest.Models.TypeConverters;
using NUnit.Framework;

namespace KzsRest.Models.Test.TypeConverters
{
    public class DivisionTypeConverterTest: BaseTest<DivisionTypeConverter>
    {
        [TestFixture]
        public class CanConvertTo: DivisionTypeConverterTest
        {
            [Test]
            public void WhenDestinationIsString_ReturnsTrue()
            {
                var actual = Target.CanConvertTo(null, typeof(string));

                Assert.That(actual, Is.True);
            }
        }
        [TestFixture]
        public class ConvertTo: DivisionTypeConverterTest
        {
            [TestCase(DivisionType.First, ExpectedResult = "1")]
            [TestCase(DivisionType.FirstA, ExpectedResult = "1a")]
            [TestCase(DivisionType.FirstB, ExpectedResult = "1b")]
            [TestCase(DivisionType.FirstQualify, ExpectedResult = "1q")]
            [TestCase(DivisionType.Second, ExpectedResult = "2")]
            [TestCase(DivisionType.SecondA, ExpectedResult = "2a")]
            [TestCase(DivisionType.SecondB, ExpectedResult = "2b")]
            public string ConvertsCorrectly(DivisionType division)
            {
                return (string)Target.ConvertTo(division, typeof(string));
            }
        }
    }
}
