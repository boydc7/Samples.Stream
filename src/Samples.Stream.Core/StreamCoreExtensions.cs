using Microsoft.Extensions.DependencyInjection;
using Samples.Contracts.Stream.Interfaces;

namespace Samples.Stream.Core;

public static class StreamCoreExtensions
{
    private static bool _hostedServiceAdded;

    public static IServiceCollection AddSimpleSnippetProcessingServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ISnippetProcessingService, SimpleSnippetProcessingService>()
                         .AddSingleton<ISnippetStatsService, SimpleSnippetStatsService>();

        AddHostedService(serviceCollection);

        return serviceCollection;
    }

    private static void AddHostedService(IServiceCollection serviceCollection)
    {
        if (_hostedServiceAdded)
        {
            return;
        }

        _hostedServiceAdded = true;

        serviceCollection.AddHostedService<SnippetProcessingHostedService>();
    }
}
