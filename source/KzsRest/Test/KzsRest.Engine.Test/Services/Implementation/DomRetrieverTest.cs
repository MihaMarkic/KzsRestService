using AutoFixture;
using KzsRest.Engine.Services.Abstract;
using KzsRest.Engine.Services.Implementation;
using KzsRest.Services.Implementation;
using NUnit.Framework;
using System;
using System.Text;

namespace KzsRest.Engine.Test.Services.Implementation
{
    public class DomRetrieverTest: BaseTest<DomRetriever>
    {
        [SetUp]
        public new void SetUp()
        {
            fixture.Register<IConvert>(() => new KzsConvert());
        }
        internal string ConvertToBase64(string source)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(source));
        }
        [TestFixture]
        public class ParseItem : DomRetrieverTest
        {
            [Test]
            public void WhenSingleContentAndStartsAtBeginning_ParsesCorrectly()
            {
                
                string source = $"#Root#{ConvertToBase64("Test")}";

                var actual = Target.ParseItem(source, 0);

                Assert.That(actual.Result.Value, Is.EqualTo(new DomResultItem("Root", "Test")));
            }
            [Test]
            public void WhenRandomTextBeforeRootContentAndStartsAtBeginning_ParsesCorrectly()
            {

                string source = $"Bu bla bla #Root#{ConvertToBase64("Test")}";

                var actual = Target.ParseItem(source, 0);

                Assert.That(actual.Result.Value, Is.EqualTo(new DomResultItem("Root", "Test")));
            }
            [Test]
            public void WhenSingleContentAndStartsAtBeginning_IndexEndOfText()
            {
                string source = $"#Root#{ConvertToBase64("Test")}#";

                var actual = Target.ParseItem(source, 0);

                Assert.That(actual.Index, Is.EqualTo(source.Length));
            }
            [Test]
            public void WhenNoData_ReturnsNullResult()
            {
                var actual = Target.ParseItem("", 0);

                Assert.That(actual.Result.HasValue, Is.False);
            }
            [Test]
            public void WhenTwoContentsAndStartsAtBeginning_ParsesFirst()
            {
                string source = $"#Root#{ConvertToBase64("Test")}#Second#{ConvertToBase64("SecondTest")}";

                var actual = Target.ParseItem(source, 0);

                Assert.That(actual.Result.Value, Is.EqualTo(new DomResultItem("Root", "Test")));
            }
            [Test]
            public void WhenTwoContentsAndStartsAtSecond_MovesIndexToEndOfFirstContent()
            {
                string source1 = $"#Root#{ConvertToBase64("Test")}#";
                string source2 = $"#Second#{ConvertToBase64("SecondTest")}#";

                var actual = Target.ParseItem(source1 + source2, 0);

                Assert.That(actual.Index, Is.EqualTo(source1.Length));
            }
            [Test]
            public void WhenTwoContentsAndStartsAtSecond_ParsesSecond()
            {
                string source1 = $"#Root#{ConvertToBase64("Test")}";
                string source2 = $"#Second#{ConvertToBase64("SecondTest")}";

                var actual = Target.ParseItem(source1 + source2, source1.Length);

                Assert.That(actual.Result.Value, Is.EqualTo(new DomResultItem("Second", "SecondTest")));
            }
            [Test]
            public void WhenEmptyContentAndNoNextBlock_ReturnsIndexNegative()
            {
                const string source = "#Root#";

                var actual = Target.ParseItem(source, 0);

                Assert.That(actual.Index, Is.LessThan(0));
            }
        }
        [TestFixture]
        public class ParseResult: DomRetrieverTest
        {
            [Test]
            public void WhenSingleContent_ParsesCorrectly()
            {

                string source = $"#Root#{ConvertToBase64("Test")}#";

                var actual = Target.ParseResult(source);

                Assert.That(actual.Length, Is.EqualTo(1));
                Assert.That(actual[0], Is.EqualTo(new DomResultItem("Root", "Test")));
            }
            [Test]
            public void WhenTwoContents_ParsesBoth()
            {
                string source = $"#Root#{ConvertToBase64("Test")}##Second#{ConvertToBase64("SecondTest")}#";

                var actual = Target.ParseResult(source);

                Assert.That(actual.Length, Is.EqualTo(2));
                Assert.That(actual[0], Is.EqualTo(new DomResultItem("Root", "Test")));
                Assert.That(actual[1], Is.EqualTo(new DomResultItem("Second", "SecondTest")));
            }
        }
    }
}
