using Microsoft.AspNetCore.Mvc;
using Samples.Api.Stream.Models;
using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;
using Samples.Metrics.Abstractions;

namespace Samples.Api.Stream.Controllers;

public class SnippetsController : StreamControllerBase
{
    private readonly ISnippetIngestionService _snippetIngestionService;
    private readonly ITransform<ApiSnippet, Snippet> _transform;
    private readonly IStatsManager _statsManager;

    public SnippetsController(ISnippetIngestionService snippetIngestionService,
                              ITransform<ApiSnippet, Snippet> transform,
                              IStatsManager statsManager)
    {
        _snippetIngestionService = snippetIngestionService;
        _transform = transform;
        _statsManager = statsManager;
    }

    [HttpPost]
    public async Task<NoContentResult> Post([FromBody] IReadOnlyCollection<ApiSnippet> request)
    {
        foreach (var apiSnippet in request)
        {
            await PostOneSnippet(apiSnippet);
        }

        _statsManager.Increment(SamplesMetrics.ApiSnippetsPosted, request.Count);

        return NoContent();
    }

    private async ValueTask PostOneSnippet(ApiSnippet apiSnippet)
    {
        var snippet = _transform.Transform(apiSnippet);

        await _snippetIngestionService.IngestAsync(snippet);
    }
}
