using KzsRest.Engine.MetricsExtensions;
using KzsRest.Engine.Services.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    public class FileHtmlSource : IDomSource
    {
        const string Root = "http://www.kzs.si/";
        readonly ILogger<FileHtmlSource> logger;
        readonly ISystem system;
        readonly IHttpContextAccessor httpContextAccessor;
        public FileHtmlSource(ILogger<FileHtmlSource> logger, ISystem system, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.system = system;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> GetHtmlContentAsync(string relativeAddress, CancellationToken ct, params string[] args)
        {
            await GetUrlContentAsync(ct);
            var exeRoot = Path.GetDirectoryName(typeof(FileHtmlSource).Assembly.Location);
            string file = Path.Combine(exeRoot, "test_html", MakeFilename(relativeAddress));
            if (File.Exists(file))
            {
                return File.ReadAllText(file);
            }
            else
            {
                throw new Exception($"No content for {file}");
            }
        }
        internal static string MakeFilename(string address)
        {
            return address.Replace("/", "_").Replace(".", "_").Replace("#", "_").Replace("&", "_").Replace(":", "_").Replace("$", "_");
        }

        public Task<string> GetUrlContentAsync(CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<string>();
            AppMetrics.DomRequestsTotal.Labels(httpContextAccessor.HttpContext.Request.Path).Inc();
            var exeRoot = Path.GetDirectoryName(typeof(DomRetriever).Assembly.Location);
#if DEBUG
            string exeName = "phantomjs.exe";
#else
                string exeName = "phantomjs";
#endif
            var psi = new ProcessStartInfo(Path.Combine(exeRoot, exeName))
            {
                UseShellExecute = false,
                WorkingDirectory = exeRoot,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            string address = "https://google.com/";
            string jsRoot = Path.Combine(system.ContentRootPath, "Content", "js");
            psi.Arguments = $"--debug=false --cookies-file=\"{Path.Combine(exeRoot, "cookies.dat")}\"--disk-cache=true --disk-cache-path=\"{Path.Combine(exeRoot, "cache")}\" --load-images=false \"{Path.Combine(jsRoot, "load_and_click.js")}\" \"{address}\"";
            //logger.LogInformation($"Phantom is: {psi.FileName}");
            //logger.LogInformation($"Phantom arguments: {psi.Arguments}");
            //logger.LogInformation(psi.Arguments);
            var process = new Process()
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };
            string result = "";

            EventHandler exited = null;
            exited = (s, e) =>
            {
                process.Exited -= exited;
                tcs.SetResult(result);
                process.Dispose();
            };
            process.Exited += exited;
            process.OutputDataReceived += (s, e) => result += e.Data;
            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    logger.LogWarning($"PhantomJS: {e.Data}");
                }
            };
            try
            {
                //using (var stopWatch = new HistogramStopwatch(AppMetrics.DomRequestsDuration.Labels(httpContextAccessor.HttpContext.Request.Path)))
                    process.Start();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to start phantomjs");
                throw;
            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            return tcs.Task;
            // times out after 30s
        }
    }
}
