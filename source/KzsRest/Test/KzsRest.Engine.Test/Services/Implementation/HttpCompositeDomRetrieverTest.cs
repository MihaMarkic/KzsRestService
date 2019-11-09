using KzsRest.Engine.Services.Implementation;
using NUnit.Framework;
using System.IO;

namespace KzsRest.Engine.Test.Services.Implementation
{
    public class HttpCompositeDomRetrieverTest : BaseTest<HttpCompositeDomRetriever>
    {
        [TestFixture]
        public class ExtractDom: HttpCompositeDomRetrieverTest
        {
            internal static string GetSampleContent(string file) => File.ReadAllText(Path.Combine("Samples", $"{file}"));
            [Test]
            public void ExtractsDataCorrectly()
            {
                string source = GetSampleContent("sample_widget_api_response_content.js");

                string actual = HttpCompositeDomRetriever.ExtractDom(source);

                Assert.That(actual.StartsWith("<table id=\"mbt-v2-schedule-table\" "));
                Assert.That(actual.EndsWith("</script>"));
            }
        }
        [TestFixture]
        public class ReplaceSpecialCharacters: HttpCompositeDomRetrieverTest
        {
            [TestCase(@"\""", ExpectedResult = @"""")]
            public string Test(string source)
            {
                return HttpCompositeDomRetriever.ReplaceSpecialCharacters(source);
            }
        }
    }
}
