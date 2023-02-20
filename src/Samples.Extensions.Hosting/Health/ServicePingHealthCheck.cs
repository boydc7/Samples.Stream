using Microsoft.Extensions.Diagnostics.HealthChecks;
using Samples.Configuration.Exceptions;

namespace Samples.Extensions.Hosting.Health;

internal class ServicePingHealthCheck : IHealthCheck
{
    private ServicePingHealthCheck() { }

    public static ServicePingHealthCheck Instance { get; } = new();

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        => Task.FromResult(SampleApplicationShutdownCancellationSource.Instance.Token.IsCancellationRequested
                               ? HealthCheckResult.Unhealthy("InShutdown", new ApplicationInShutdownException())
                               : HealthCheckResult.Healthy("OK"));
}
