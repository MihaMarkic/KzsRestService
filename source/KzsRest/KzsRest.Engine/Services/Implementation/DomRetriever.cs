using KzsRest.Engine.Services.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    /// <summary>
    /// Extract pages from phantomjs.
    /// Requires Content/js/load_and_click.js, base64.min.js and text-encoder-lite.js
    /// </summary>
    public class DomRetriever : IDomRetriever
    {
        readonly ILogger<KzsParser> logger;
        readonly IDomSource domSource;
        readonly IConvert convert;
        public DomRetriever(IDomSource domSource, IConvert convert, ILogger<KzsParser> logger)
        {
            this.logger = logger;
            this.domSource = domSource;
            this.convert = convert;
        }
        public async Task<DomResultItem[]> GetDomForAsync(string relativeAddress, CancellationToken ct, params string[] args)
        {
            try
            {
                string content;
                // give content 10s to load
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                using (var combined = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, ct))
                {
                    content = await domSource.GetHtmlContentAsync(relativeAddress, ct, args).ConfigureAwait(false);
                }
                var items = ParseResult(content);
                return items;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("DOM retrieval was cancelled");
                return new DomResultItem[0];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DOM retrieval failed");
                return new DomResultItem[0];
            }
        }

        internal DomResultItem[] ParseResult(string data)
        {
            // should be optimized using span eventually
            // right now it's bad memory wise
            var list = new List<DomResultItem>();
            int index = 0;
            DomResultItem? result;
            while (index >= 0)
            {
                (result, index) = ParseItem(data, index);
                if (result.HasValue)
                {
                    list.Add(result.Value);
                }
            }
            return list.ToArray();

        }

        internal (DomResultItem? Result, int Index) ParseItem(string data, int index)
        {
            int headerStart = data.IndexOf('#', index);
            if (headerStart >= 0)
            {
                int headerEnd = data.IndexOf('#', headerStart + 1);
                string id = data.Substring(headerStart + 1, headerEnd - headerStart - 1);
                int contentEndHeader = data.IndexOf('#', headerEnd + 1);
                ReadOnlySpan<char> base64;
                if (contentEndHeader < 0)
                {
                    base64 = data.AsSpan().Slice(start: headerEnd + 1);
                }
                else
                {
                    base64 = data.AsSpan().Slice(start: headerEnd + 1, length: contentEndHeader - headerEnd - 1);
                }
                int minLength = GetBufferForBase64Length(base64);
                var buffer = ArrayPool<byte>.Shared.Rent(minLength);
                try
                {
                    if (convert.TryFromBase64Chars(base64, buffer, out int bytesWritten))
                    {
                        return (new DomResultItem(id, Encoding.UTF8.GetString(buffer, 0, bytesWritten)), contentEndHeader + 1);
                    }
                    else
                    {
                        throw new Exception($"Failed converting {id}");
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
            return (null, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        /// <remarks>Taken from https://stackoverflow.com/a/51301284/84615</remarks>
        internal static int GetBufferForBase64Length(ReadOnlySpan<char> base64)
        {
            return ((base64.Length * 3) + 3) / 4 -
                (base64.Length > 0 && base64[base64.Length - 1] == '=' ?
                    base64.Length > 1 && base64[base64.Length - 2] == '=' ?
                        2 : 1 : 0);
        }
    }
}
