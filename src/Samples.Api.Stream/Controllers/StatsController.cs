using Microsoft.AspNetCore.Mvc;
using Samples.Api.Stream.Models;
using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;

namespace Samples.Api.Stream.Controllers;

public class StatsController : StreamControllerBase
{
    private readonly ISnippetStatsService _snippetStatsService;
    private readonly ITransform<ApiSnippetStatValue, SnippetStat> _transform;

    public StatsController(ISnippetStatsService snippetStatsService,
                           ITransform<ApiSnippetStatValue, SnippetStat> transform)
    {
        _snippetStatsService = snippetStatsService;
        _transform = transform;
    }

    [HttpGet]
    public async Task<ActionResult<SampleApiResults<ApiSnippetStat>>> Get()
    {
        var stats = await _snippetStatsService.GetStatsAsync();

        var result = stats.GroupBy(s => s.KeyName)
                          .Select(g => new ApiSnippetStat
                                       {
                                           KeyName = g.Key,
                                           Stats = g.Select(gs => _transform.Transform(gs))
                                                    .ToList()
                                       })
                          .AsOkSampleApiResults();

        return result;
    }
}
