using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;
using Samples.Extensions;
using StackExchange.Redis;

namespace Samples.Redis.Stream;

public class RedisCountSnippetProcessor : ISnippetProcessor
{
    private static readonly RedisKey _snippetCountKeyName = new("stream:totalcount");

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisCountSnippetProcessor(ConnectionMultiplexerFactory redisFactory)
    {
        _connectionMultiplexer = redisFactory.Snippets;
    }

    public async Task ProcessAsync(Snippet snippet)
    {
        var db = _connectionMultiplexer.GetDatabase();

        await db.StringIncrementAsync(_snippetCountKeyName);
    }

    public async Task<ICollection<SnippetStat>> GetValuesAsync()
    {
        var db = _connectionMultiplexer.GetDatabase();

        var val = await db.StringGetAsync(_snippetCountKeyName);

        return new[]
               {
                   new SnippetStat
                   {
                       KeyName = _snippetCountKeyName,
                       StatName = "Total",
                       StatValue = val.ToString().ToLong()
                   }
               };
    }
}
