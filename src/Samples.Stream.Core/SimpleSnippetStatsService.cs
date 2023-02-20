using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;
using Samples.Metrics.Abstractions;

namespace Samples.Stream.Core;

public class SimpleSnippetStatsService : ISnippetStatsService
{
    private readonly IStatsManager _statsManager;
    private readonly IReadOnlyList<ISnippetProcessor> _processors;

    public SimpleSnippetStatsService(IEnumerable<ISnippetProcessor> processors,
                                     IStatsManager statsManager)
    {
        _statsManager = statsManager;
        _processors = (processors ?? throw new ArgumentNullException(nameof(processors))).ToList();
    }

    public async Task<IReadOnlyCollection<SnippetStat>> GetStatsAsync()
    {
        var tasks = _processors.Select(p => p.GetValuesAsync()).ToArray();

        await Task.WhenAll(tasks);

        var results = tasks.SelectMany(t => t.Result).ToList();

        _statsManager.Increment(SamplesMetrics.ProcessingStatsCollected, results.Count);

        return results;
    }
}
