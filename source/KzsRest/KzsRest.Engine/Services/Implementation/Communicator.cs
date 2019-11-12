using KzsRest.Engine.Services.Abstract;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Engine.Services.Implementation
{
    public class Communicator : DisposableObject, ICommunicator
    {
        readonly HttpClient client;
        readonly HttpClientHandler handler;
        public Communicator()
        {
            handler = new HttpClientHandler { CookieContainer = new CookieContainer() };
            client = new HttpClient(handler);
        }
        public async Task<string> GetResponseAsync(string uri, CancellationToken ct)
        {
            var response = await client.GetAsync(uri, ct);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"Failed loading {uri}: {response.ReasonPhrase}");
            }
        }
        protected override void Dispose(bool disposing)
        {
            client.CancelPendingRequests();
            client.Dispose();
            base.Dispose(disposing);
        }
    }
}
