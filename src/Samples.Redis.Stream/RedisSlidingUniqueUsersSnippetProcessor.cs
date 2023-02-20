using Samples.Contracts.Stream.Interfaces;
using Samples.Contracts.Stream.Models;
using Samples.Extensions;
using StackExchange.Redis;

namespace Samples.Redis.Stream;

public class RedisSlidingUniqueUsersSnippetProcessor : ISnippetProcessor
{
    private const string _keyPrefix = "stream:uniqueusers:";

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisSlidingUniqueUsersSnippetProcessor(ConnectionMultiplexerFactory redisFactory)
    {
        _connectionMultiplexer = redisFactory.Snippets;
    }

    public async Task ProcessAsync(Snippet snippet)
    {
        var db = _connectionMultiplexer.GetDatabase();

        var minuteBucketKey = snippet.CreatedOnUtc.ToMinuteBucketKey(1);

        var key = string.Concat(_keyPrefix, minuteBucketKey);

        var keyAdded = await db.HyperLogLogAddAsync(key, snippet.UserId);

        if (keyAdded)
        {
            await db.KeyExpireAsync(key, TimeSpan.FromMinutes(16));
        }
    }

    public async Task<ICollection<SnippetStat>> GetValuesAsync()
    {
        const int minutesBackToCollect = 5;

        var db = _connectionMultiplexer.GetDatabase();

        var currentBucketTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToMinuteBucketKey(1);
        var uniqueUsers = new List<SnippetStat>(minutesBackToCollect);
        
        for (var x = 1; x <= minutesBackToCollect; x++)
        {
            var key = string.Concat(_keyPrefix, currentBucketTimestamp);

            var val = await db.HyperLogLogLengthAsync(key);

            uniqueUsers.Add(new SnippetStat
                            {
                                KeyName = _keyPrefix,
                                StatName = key,
                                StatValue = val.ToString().ToLong()
                            });
            
            // Take 60 seconds off the current bucket to get the prior bucket
            currentBucketTimestamp -= 60;
        }

        return uniqueUsers;
    }
}
