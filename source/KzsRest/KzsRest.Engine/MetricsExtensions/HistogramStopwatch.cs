using Prometheus;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace KzsRest.Engine.MetricsExtensions
{
    public class HistogramStopwatch : DisposableObject
    {
        private readonly IHistogram histogram;
        private readonly Stopwatch watch;
        private readonly bool useTicks;
        public HistogramStopwatch(IHistogram histogram, bool useTicks = false)
        {
            Contract.Requires(histogram != null, nameof(histogram) + " is null.");
            this.histogram = histogram;
            this.useTicks = useTicks;
            watch = Stopwatch.StartNew();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                histogram.Observe(useTicks ? watch.ElapsedTicks : watch.ElapsedMilliseconds);
            }
            base.Dispose(disposing);
        }
    }
}
