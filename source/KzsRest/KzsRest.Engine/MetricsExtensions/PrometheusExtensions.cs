using Prometheus;

namespace KzsRest.Engine.MetricsExtensions
{
    public static class PrometheusExtensions
    {
        public static HistogramStopwatch CreateStopwatch(this IHistogram histogram, bool useTicks = false)
        {
            return new HistogramStopwatch(histogram, useTicks);
        }
    }
}
