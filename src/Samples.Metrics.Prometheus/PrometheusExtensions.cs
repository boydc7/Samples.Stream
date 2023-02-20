using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Samples.Metrics.Abstractions;

namespace Samples.Metrics.Prometheus;

public static class PrometheusExtensions
{
    private static bool _configBuilt;

    public static IApplicationBuilder UsePrometheusMetrics(this IApplicationBuilder applicationBuilder)
    {
        if (_configBuilt)
        {
            return applicationBuilder;
        }

        _configBuilt = true;

        applicationBuilder.UseEndpoints(b => b.MapMetrics());

        return applicationBuilder;
    }

    public static IServiceCollection AddPrometheusMetrics(this IServiceCollection serviceCollection)
        => serviceCollection.AddSingleton<IStatsManager, PrometheusStatsManager>();
}
