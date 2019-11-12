using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    public class SessionRetrieverHttpClient: DisposableObject
    {
        const string SessionIdKey = "mbt-widget-appseason_id";
        readonly HttpClient client;
        readonly HttpClientHandler handler;
        readonly CookieContainer cookieContainer;
        public SessionRetrieverHttpClient()
        {
            cookieContainer = new CookieContainer();
            handler = new HttpClientHandler { CookieContainer = cookieContainer };
            client = new HttpClient(handler);
        }

        public async Task<int?> GetSessionIdAsync(Uri uri, CancellationToken ct)
        {
            for (int i = 0; i < 2; i++)
            { 
                var response = await client.GetAsync(uri, ct);
                if (response.IsSuccessStatusCode)
                {
                    var cookies = cookieContainer.GetCookies(uri);
                    string sessionText = cookies[SessionIdKey]?.Value;
                    if (!string.IsNullOrWhiteSpace(sessionText))
                    {
                        if (int.TryParse(sessionText, out int sessionId))
                        {
                            return sessionId;
                        }
                    }
                }
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            client.CancelPendingRequests();
            client.Dispose();
            base.Dispose(disposing);
        }
    }
}
