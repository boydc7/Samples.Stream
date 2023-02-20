using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Samples.Redis.Stream;

public class ConnectionMultiplexerFactory
{
    private static readonly object _lockObject = new();
    private static volatile ConnectionMultiplexerFactory _instance;

    private readonly IConfiguration _configuration;

    private ConnectionMultiplexer _snippets;

    private ConnectionMultiplexerFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static ConnectionMultiplexerFactory Create(IConfiguration configuration)
    {
        if (_instance != null)
        {
            return _instance;
        }

        lock(_lockObject)
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new ConnectionMultiplexerFactory(configuration);
        }

        return _instance;
    }

    public ConnectionMultiplexer Snippets => _snippets ??= ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis") ?? throw new ArgumentNullException(nameof(_configuration)));
}
