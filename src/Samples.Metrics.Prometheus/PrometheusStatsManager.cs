using System.Collections.Concurrent;
using Prometheus;
using Samples.Extensions;
using Samples.Metrics.Abstractions;

namespace Samples.Metrics.Prometheus;

public class PrometheusStatsManager : IStatsManager
{
    private static readonly string[] _labels =
    {
        "samples",
        "stream",
        "samples_stream"
    };

    private static readonly CounterConfiguration _defaultConfig = new()
                                                                  {
                                                                      ExemplarBehavior = ExemplarBehavior.NoExemplars(),
                                                                      LabelNames = _labels
                                                                  };

    private readonly ConcurrentDictionary<string, Counter> _counters = new();

    public void Increment(string key, double incrementBy = 1)
    {
        if (key.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(key));
        }

        var counter = _counters.GetOrAdd(key, k => global::Prometheus.Metrics.CreateCounter(k, k, _defaultConfig));

        counter.Inc(incrementBy);
    }
}
