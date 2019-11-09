using KzsRest.Engine.Services.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace KzsRest.Engine.Services.Implementation
{
    /// <summary>
    /// Extract pages from phantomjs.
    /// Requires Content/js/load_and_click.js, base64.min.js and text-encoder-lite.js
    /// </summary>
    public class DomRetriever : DisposableObject, IDomRetriever
    {
        const int NumberOfConsumers = 3;
        readonly ILogger<KzsStructureParser> logger;
        readonly IConvert convert;
        readonly BufferBlock<HtmlRequest> buffer;
        readonly CancellationTokenSource cts;
        readonly Task[] consumer;
        public DomRetriever(Func<IDomSource> domSourceFactory, IConvert convert, ILogger<KzsStructureParser> logger)
        {
            this.logger = logger;
            this.convert = convert;
            buffer = new BufferBlock<HtmlRequest>();
            cts = new CancellationTokenSource();
            consumer = new Task[NumberOfConsumers];
            for (int i = 0; i < NumberOfConsumers; i++)
            {
                int id = i;
                var domSource = domSourceFactory();
                consumer[i] = Task.Run(() => ConsumerAsync(id, domSource, cts.Token));
            }
        }
        public async Task<DomResultItem[]> GetDomForAsync(string relativeAddress, CancellationToken ct, params string[] args)
        {
            var htmlRequest = new HtmlRequest(relativeAddress, args);
            try
            {
                await buffer.SendAsync(htmlRequest);
                string content = await htmlRequest.Task.ConfigureAwait(false);
                logger.LogInformation($"Received HTML for {htmlRequest.RelativeAddress}");
                var items = ParseResult(content);
                logger.LogInformation($"Parsed HTML for {htmlRequest.RelativeAddress}");
                return items;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning($"Html retrieval for {htmlRequest.RelativeAddress} was cancelled");
                return new DomResultItem[0];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Html retrieval for {htmlRequest.RelativeAddress} failed");
                return new DomResultItem[0];
            }
        }
        internal async Task ConsumerAsync(int id, IDomSource domSource, CancellationToken ct)
        {
            logger.LogInformation($"Consumer {id} started");
            try
            {
                while (await buffer.OutputAvailableAsync(ct))
                {
                    while (buffer.TryReceive(out var htmlRequest))
                    {
                        try
                        {
                            logger.LogInformation($"Consumer {id} will process {htmlRequest.RelativeAddress}");
                            using (var timeConstraint = new CancellationTokenSource(TimeSpan.FromSeconds(20)))
                            using (var combined = CancellationTokenSource.CreateLinkedTokenSource(timeConstraint.Token, ct))
                            {
                                string content = await domSource.GetHtmlContentAsync(htmlRequest.RelativeAddress, combined.Token, htmlRequest.Args);
                                logger.LogInformation($"Consumer {id} setting completed for {htmlRequest.RelativeAddress}");
                                htmlRequest.SetCompleted(content);
                                logger.LogInformation($"Consumer {id} set completed for {htmlRequest.RelativeAddress}");
                            }
                            logger.LogInformation($"Tokens disposed for consumer {id} addresss: {htmlRequest.RelativeAddress}");
                        }
                        catch (OperationCanceledException)
                        {
                            logger.LogInformation($"Consumer processing was cancelled for htmlRequest to {htmlRequest.RelativeAddress}");
                            htmlRequest.SetCanceled();
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning(ex, $"Consumer failed processing htmlRequest for {htmlRequest.RelativeAddress}");
                            htmlRequest.SetException(ex);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Consumer was cancelled");
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
                logger.LogInformation($"Parsing index {index}");
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
                logger.LogInformation($"Min length for {index} is {minLength}");
                var buffer = ArrayPool<byte>.Shared.Rent(minLength);
                logger.LogInformation($"Buffer acquired for {index}:{buffer.GetHashCode()}");
                try
                {
                    if (convert.TryFromBase64Chars(base64, buffer, out int bytesWritten))
                    {
                        return (new DomResultItem(id, Encoding.UTF8.GetString(buffer, 0, bytesWritten)), 
                            contentEndHeader < 0 ? contentEndHeader : contentEndHeader + 1);
                    }
                    else
                    {
                        throw new Exception($"Failed converting {id}");
                    }
                }
                finally
                {
                    logger.LogInformation($"Buffer releasing for {index}:{buffer.GetHashCode()}");
                    ArrayPool<byte>.Shared.Return(buffer);
                    logger.LogInformation($"Buffer released for {index}:{buffer.GetHashCode()}");
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                logger.LogInformation("Disposing");
                cts.Cancel();
            }
            base.Dispose(disposing);
        }
    }
}
