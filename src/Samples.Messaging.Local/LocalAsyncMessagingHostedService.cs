using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Samples.Messaging.Abstractions;

namespace Samples.Messaging.Local;

public class LocalAsyncMessagingHostedService : IHostedService
{
    private readonly ILocalProducer _localProducer;
    private readonly ILogger<LocalAsyncMessagingHostedService> _log;

    public LocalAsyncMessagingHostedService(ILocalProducer localProducer, ILogger<LocalAsyncMessagingHostedService> log)
    {
        _localProducer = localProducer;
        _log = log;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _localProducer.Init();

        _log.LogInformation("...Startup completed for {ClassName}...", nameof(LocalAsyncMessagingHostedService));

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _localProducer.OnShutdown();

        _log.LogInformation("...Shutdown started for {ClassName}...", nameof(LocalAsyncMessagingHostedService));

        while (_localProducer.WorkerCount > 0 && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(250, cancellationToken);
        }

        _log.LogInformation("...Shutdown completed for {ClassName}...", nameof(LocalAsyncMessagingHostedService));
    }
}
