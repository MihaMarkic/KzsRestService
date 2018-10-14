using Prometheus;

namespace KzsRest.Engine.MetricsExtensions
{
    public static class AppMetrics
    {
        public readonly static Counter RequestsTotal = Metrics.CreateCounter("requests_total", "Total number of requests", "path");
        public readonly static Counter RequestsFailuresTotal = Metrics.CreateCounter("requests_failures_total", "Total number of failed requests", "path");
        public readonly static Counter CacheHits = Metrics.CreateCounter("cache_hits_total", "Total number of cache hits", "path");
        public readonly static Histogram RequestProcessDuration = Metrics.CreateHistogram("full_requests_process_duration_in_seconds", 
            "Request that doesn't get data from cache duration in seconds", null, "path");
        //public readonly static Histogram CachedRequestProcessDuration = Metrics.CreateHistogram("full_requests_from_cache_process_duration_in_seconds", 
        //    "Request that get data from cache duration in seconds", null, "path");
        public readonly static Counter DomRequestsTotal = Metrics.CreateCounter("dom_requests_total", "Total number of requests to source server", "path");
        public readonly static Histogram DomRequestsDuration = Metrics.CreateHistogram("dom_requests_duration_in_seconds", 
            "Request to source server duration in seconds", null, "path");
    }
}
