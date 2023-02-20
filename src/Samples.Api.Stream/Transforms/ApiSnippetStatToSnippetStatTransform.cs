using Samples.Api.Stream.Models;
using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;

namespace Samples.Api.Stream.Transforms;

public class ApiSnippetStatValueToSnippetStatTransform : ITransform<ApiSnippetStatValue, SnippetStat>
{
    public SnippetStat Transform(ApiSnippetStatValue source)
        => new()
           {
               KeyName = source.StatName,
               StatName = source.StatName,
               StatValue = source.StatValue
           };

    public ApiSnippetStatValue Transform(SnippetStat source)
        => new()
           {
               StatName = source.StatName,
               StatValue = source.StatValue
           };
}
