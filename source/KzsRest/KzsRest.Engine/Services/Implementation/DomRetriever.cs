using Flurl;
using KzsRest.Engine.Models;
using KzsRest.Engine.Services.Abstract;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        const string Root = "http://www.kzs.si/";
        readonly IConvert convert;
        readonly ISystem system;
        readonly ILogger<KzsParser> logger;
        public DomRetriever(IConvert convert, ISystem system, ILogger<KzsParser> logger)
        {
            this.convert = convert;
            this.system = system;
            this.logger = logger;
        }
        public Task<DomResultItem[]> GetDomForAsync(string relativeAddress, CancellationToken ct, params string[] args)
        {
            return Task.Run(() =>
            {
                var exeRoot = Path.GetDirectoryName(typeof(DomRetriever).Assembly.Location);
                var psi = new ProcessStartInfo(Path.Combine(exeRoot, "phantomjs.exe"))
                {
                    UseShellExecute = false,
                    WorkingDirectory = exeRoot,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                string address = Url.Combine(Root, relativeAddress);
                string jsRoot = Path.Combine(system.ContentRootPath, "Content", "js");
                psi.Arguments = $"--debug=false --cookies-file=\"{Path.Combine(exeRoot, "cookies.dat")}\"--disk-cache=true --disk-cache-path=\"{Path.Combine(exeRoot, "cache")}\" --load-images=false \"{Path.Combine(jsRoot, "load_and_click.js")}\" \"{address}\" {string.Join(" ", args)}";
                logger.LogInformation(psi.Arguments);
                var process = new Process()
                {
                    StartInfo = psi,
                    EnableRaisingEvents = true
                };
                string result = "";
                process.OutputDataReceived += (s, e) => result += e.Data;
                process.ErrorDataReceived += (s, e) => Debug.WriteLine(e.Data);
                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to start phantomjs");
                    return new DomResultItem[0];
                }
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                // times out after 30s
                if (process.WaitForExit(30 * 1000))
                {
                    var items = ParseResult(result);
                    return items;
                }
                else
                {
                    logger.LogWarning("Timeout while waiting for phantom");
                    return new DomResultItem[0];
                }
            });
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
