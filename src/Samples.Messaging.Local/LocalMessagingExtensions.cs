using Microsoft.Extensions.DependencyInjection;
using Samples.Messaging.Abstractions;

namespace Samples.Messaging.Local;

public static class LocalMessagingExtensions
{
    public static IServiceCollection AddSampleLocalDeferredProducerConsumer(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<ILocalBuffer<LocalDeferred>, InMemoryBuffer<LocalDeferred>>()
                            .AddSingleton<ILocalProducer, LocalAsyncProducer<LocalDeferred>>()
                            .AddHostedService<LocalAsyncMessagingHostedService>();
}
