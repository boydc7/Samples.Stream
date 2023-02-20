namespace Samples.Metrics.Abstractions;

public interface IStatsManager
{
    void Increment(string key, double incrementBy = 1);
}
