using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Samples.Contracts.Stream.Interfaces;

namespace Samples.Redis.Stream;

public static class StreamRedisExtensions
{
    public static IServiceCollection AddStreamRedisSnippetProcessors(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton(s => ConnectionMultiplexerFactory.Create(s.GetRequiredService<IConfiguration>()));

        serviceCollection.AddSingleton<ISnippetProcessor, RedisTopHashtagsSnippetProcessor>()
                         .AddSingleton<ISnippetProcessor, RedisCountSnippetProcessor>()
                         .AddSingleton<ISnippetProcessor, RedisSlidingUniqueUsersSnippetProcessor>();

        return serviceCollection;
    }
}
