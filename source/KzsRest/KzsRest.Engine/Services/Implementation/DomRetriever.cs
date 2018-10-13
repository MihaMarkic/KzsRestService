using Flurl;
using KzsRest.Engine.Models;
using KzsRest.Engine.Services.Abstract;
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
    public class DomRetriever : IDomRetriever
    {
        const string Root = "https://www.kzs.si/";
        readonly IConvert convert;
        public DomRetriever(IConvert convert)
        {
            this.convert = convert;
        }
        public Task<DomResultItem[]> GetDomForAsync(string relativeAddress, CancellationToken ct, params string[] args)
        {
            return Task.Run(() =>
            {
                var psi = new ProcessStartInfo(@"D:\Utilities\phantomjs-2.1.1-windows\bin\phantomjs.exe")
                {
                    UseShellExecute = false,
                    WorkingDirectory = @"D:\Utilities\phantomjs-2.1.1-windows\bin\",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };
                var exeRoot = Path.GetDirectoryName(typeof(DomRetriever).Assembly.Location);
                string address = Url.Combine(Root, "clanek/Tekmovanja/Mlajse-kategorije/Fantje/Fantje-U17/cid/100");
                psi.Arguments = $"--debug=false --cookies-file={Path.Combine(exeRoot, "cookies.dat")} --disk-cache=true --disk-cache-path={Path.Combine(exeRoot, "cache")} --load-images=false ./load_and_click.js {address} {string.Join(" ", args)}";
                var process = new Process()
                {
                    StartInfo = psi,
                    EnableRaisingEvents = true
                };
                string result = "";
                process.OutputDataReceived += (s, e) => result += e.Data;
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
                var items = ParseResult(result);
                return items;
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
                int headerEnd = data.IndexOf('#', index + 1);
                string id = data.Substring(headerStart + 1, headerEnd - headerStart - 1);
                int nextHeader = data.IndexOf('#', headerEnd + 1);
                ReadOnlySpan<char> base64;
                if (nextHeader < 0)
                {
                    base64 = data.AsSpan().Slice(start: headerEnd + 1);
                }
                else
                {
                    base64 = data.AsSpan().Slice(start: headerEnd + 1, length: nextHeader - headerEnd - 1);
                }
                int minLength = GetBufferForBase64Length(base64);
                var buffer = ArrayPool<byte>.Shared.Rent(minLength);
                try
                {
                    if (convert.TryFromBase64Chars(base64, buffer, out int bytesWritten))
                    {
                        return (new DomResultItem(id, Encoding.UTF8.GetString(buffer, 0, bytesWritten)), nextHeader);
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
