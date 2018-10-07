using Flurl;
using KzsRest.Engine.Services.Abstract;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    public class DomRetriever : IDomRetriever
    {
        const string Root = "https://www.kzs.si/";
        public Task<string> GetDomForAsync(string relativeAddress, CancellationToken ct)
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
                psi.Arguments = $"--debug=true --cookies-file={Path.Combine(exeRoot, "cookies.dat")} --disk-cache=true --disk-cache-path={Path.Combine(exeRoot, "cache")} --load-images=false ./test.js {relativeAddress}";
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
                return result;
            });
        }
    }
}
