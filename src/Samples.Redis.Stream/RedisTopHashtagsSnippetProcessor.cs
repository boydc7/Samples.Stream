using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;
using StackExchange.Redis;

namespace Samples.Redis.Stream;

public class RedisTopHashtagsSnippetProcessor : ISnippetProcessor
{
    private static readonly RedisKey _topHashtagsKeyName = new("stream:tophashtags");

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisTopHashtagsSnippetProcessor(ConnectionMultiplexerFactory redisFactory)
    {
        _connectionMultiplexer = redisFactory.Snippets;
    }

    public async Task ProcessAsync(Snippet snippet)
    {
        if (snippet?.Tags == null || snippet.Tags.Count == 0)
        {
            return;
        }

        var db = _connectionMultiplexer.GetDatabase();

        var tasks = snippet.Tags
                           .Select(t => db.SortedSetIncrementAsync(_topHashtagsKeyName, t, 1))
                           .ToArray();

        await Task.WhenAll(tasks);
    }

    public async Task<ICollection<SnippetStat>> GetValuesAsync()
    {
        const int lastIndexToCollecgt = 5 - 1;

        var db = _connectionMultiplexer.GetDatabase();

        var topTags = (await db.SortedSetRangeByRankWithScoresAsync(_topHashtagsKeyName, 0, lastIndexToCollecgt, Order.Descending))
                               .Select(s => new SnippetStat
                                            {
                                                KeyName = _topHashtagsKeyName,
                                                StatName = s.Element,
                                                StatValue = s.Score
                                            })
                               .ToList();

        return topTags;
    }
}
