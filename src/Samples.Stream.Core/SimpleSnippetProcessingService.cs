using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;
using Samples.Metrics.Abstractions;

namespace Samples.Stream.Core;

public class SimpleSnippetProcessingService : ISnippetProcessingService
{
    private readonly IStatsManager _statsManager;
    private readonly IReadOnlyList<ISnippetProcessor> _processors;

    public SimpleSnippetProcessingService(IEnumerable<ISnippetProcessor> processors,
                                          IStatsManager statsManager)
    {
        _statsManager = statsManager;
        _processors = (processors ?? throw new ArgumentNullException(nameof(processors))).ToList();
    }

    public async Task ProcessAsync(Snippet snippet)
    {
        var tasks = _processors.Select(p => p.ProcessAsync(snippet)).ToArray();

        await Task.WhenAll(tasks);

        _statsManager.Increment(SamplesMetrics.ProcessingSnippetsProcessed);
    }
}
