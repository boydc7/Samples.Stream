using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Samples.Extensions.Hosting.Health;
using Samples.Extensions.Hosting.Middleware;

namespace Samples.Extensions.Hosting;

public static class HostingExtensions
{
    public static void RegisterOnApplicationStopping(this IApplicationBuilder app, Action callback)
    {
        var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        appLifetime.ApplicationStopping.Register(callback);
    }

    public static void RegisterOnApplicationStarted(this IApplicationBuilder app, Action callback)
    {
        var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        appLifetime.ApplicationStarted.Register(callback);
    }

    public static void RegisterOnApplicationStopped(this IApplicationBuilder app, Action callback)
    {
        var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        appLifetime.ApplicationStopped.Register(callback);
    }

    public static IApplicationBuilder UseSampleRequestResponseLogging(this IApplicationBuilder app)
        => app.UseMiddleware<SampleLogMiddleware>();

    public static IHealthChecksBuilder AddServicePingHealthCheck(this IHealthChecksBuilder builder, string name = null)
        => builder.Add(new HealthCheckRegistration(name ?? "SampleService", ServicePingHealthCheck.Instance, HealthStatus.Unhealthy,
                                                   Enumerable.Empty<string>(), TimeSpan.FromSeconds(7)));
}
