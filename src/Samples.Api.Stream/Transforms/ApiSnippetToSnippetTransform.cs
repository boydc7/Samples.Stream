using Samples.Api.Stream.Models;
using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;
using Samples.Extensions;

namespace Samples.Api.Stream.Transforms;

public class ApiSnippetToSnippetTransform : ITransform<ApiSnippet, Snippet>
{
    public Snippet Transform(ApiSnippet source)
        => new()
           {
               Body = source.Body,
               UserId = source.UserId,
               CreatedOnUtc = source.CreatedOnUtc > 0
                                  ? source.CreatedOnUtc
                                  : DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
               Mentions = source.Mentions.ToOrdinalStringHashSet(),
               Tags = source.Tags.ToOrdinalStringHashSet()
           };

    public ApiSnippet Transform(Snippet source)
        => new()
           {
               Body = source.Body,
               UserId = source.UserId,
               CreatedOnUtc = source.CreatedOnUtc,
               Mentions = source.Mentions,
               Tags = source.Tags
           };
}
