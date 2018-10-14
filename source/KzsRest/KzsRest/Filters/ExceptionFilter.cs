using KzsRest.Engine.MetricsExtensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KzsRest.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var request = context.HttpContext.Request;
            AppMetrics.RequestsFailuresTotal.Labels(request.Path).Inc();
            context.ExceptionHandled = false;
        }
    }
}
