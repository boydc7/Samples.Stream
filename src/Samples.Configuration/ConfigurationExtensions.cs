using Microsoft.Extensions.DependencyInjection;
using Samples.Configuration.Abstractions;

namespace Samples.Configuration;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddCancelSourceShutdownRegistrar(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<IShutdownRegistrar, CancellationSourceShutdownRegistrar>();
}
