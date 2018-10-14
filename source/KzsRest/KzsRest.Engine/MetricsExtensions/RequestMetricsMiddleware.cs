using KzsRest.Engine.MetricsExtensions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace KzsRest.Engine.MetricsExtensions
{
    public class RequestMetricsMiddleware
    {
        private readonly RequestDelegate next;

        public RequestMetricsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            AppMetrics.RequestsTotal.Labels(httpContext.Request.Path).Inc();
            using (var watch = AppMetrics.RequestProcessDuration.Labels(httpContext.Request.Path).CreateStopwatch())
            {
                // Call the next middleware delegate in the pipeline 
                await next.Invoke(httpContext);
            }
        }
    }
}
