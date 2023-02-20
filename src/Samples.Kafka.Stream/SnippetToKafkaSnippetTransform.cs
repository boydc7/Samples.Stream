using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;
using Samples.Extensions;

namespace Samples.Kafka.Stream;

public class SnippetToKafkaSnippetTransform : ITransform<Snippet, StreamSnippet>
{
    public StreamSnippet Transform(Snippet source)
    {
        var ss = new StreamSnippet
                 {
                     Body = source.Body,
                     CreatedOnUtc = source.CreatedOnUtc,
                     UserId = source.UserId
                 };

        ss.Mentions.AddRange(source.Mentions);
        ss.Tags.AddRange(source.Tags);

        return ss;
    }

    public Snippet Transform(StreamSnippet source)
        => new()
           {
               Body = source.Body,
               CreatedOnUtc = source.CreatedOnUtc,
               UserId = source.UserId,
               Mentions = source.Mentions.ToOrdinalStringHashSet(),
               Tags = source.Tags.ToOrdinalStringHashSet()
           };
}
