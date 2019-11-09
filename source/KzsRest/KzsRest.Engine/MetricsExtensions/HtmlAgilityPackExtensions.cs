namespace HtmlAgilityPack
{
    public static class HtmlAgilityPackExtensions
    {
        /// <summary>
        /// Fluent version of <see cref="HtmlDocument.LoadHtml(string)"/>.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static HtmlDocument LoadHtmlFrom(this HtmlDocument document, string content)
        {
            document.LoadHtml(content);
            return document;
        }
    }
}
