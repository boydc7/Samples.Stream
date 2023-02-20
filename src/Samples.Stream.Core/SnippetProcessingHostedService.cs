using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Samples.Contracts.Stream.Interfaces;
using Samples.Extensions;

namespace Samples.Stream.Core;

public class SnippetProcessingHostedService : IHostedService
{
    private readonly ISnippetProcessingService _snippetProcessingService;
    private readonly ISnippetReader _snippetReader;
    private readonly ILogger<SnippetProcessingHostedService> _log;
    private readonly TimeSpan _pollingInterval;
    private Timer _timer;
    private readonly object _lockObject = new();
    private bool _inShutdown;
    private bool _inHostProcessing;

    public SnippetProcessingHostedService(ISnippetProcessingService snippetProcessingService,
                                          ISnippetReader snippetReader,
                                          ILogger<SnippetProcessingHostedService> log)
    {
        _snippetProcessingService = snippetProcessingService;
        _snippetReader = snippetReader;
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _pollingInterval = TimeSpan.FromSeconds(3);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ProcessTimer, null, _pollingInterval, _pollingInterval);

        return Task.CompletedTask;
    }

    private void ProcessTimer(object state)
    {
        if (_inHostProcessing || _inShutdown)
        {
            return;
        }

#pragma warning disable 4014
        ProcessTimerAsync();
#pragma warning restore 4014
    }

    private async Task ProcessTimerAsync()
    {
        if (_inHostProcessing || _inShutdown)
        {
            return;
        }

        lock(_lockObject)
        {
            if (_inHostProcessing || _inShutdown)
            {
                return;
            }

            _inHostProcessing = true;
        }

        try
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            if (!_inShutdown)
            {
                await ProcessSnippetsAsync();
            }
        }
        finally
        {
            if (!_inShutdown)
            {
                _timer.Change(_pollingInterval, _pollingInterval);
            }

            _inHostProcessing = false;
        }
    }

    public async Task ProcessSnippetsAsync()
    {
        var count = 0L;
        
        try
        {
            _log.LogInformation("Starting Snippet stream processing");
            
            foreach (var consumedSnippet in _snippetReader.Read())
            {
                await _snippetProcessingService.ProcessAsync(consumedSnippet.Event);

                _snippetReader.AckConsume(consumedSnippet.Partition, consumedSnippet.Offset);

                count++;
            }
        }
        catch(TaskCanceledException) { }
        catch(Exception x)
        {
            _log.Exception(x);
        }
        finally
        {
            _log.LogInformation("Ending Snippet stream processing - processed [{SnippetsProcessed}] snippets", count);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _inShutdown = true;

        _snippetReader.Stop();

        return Task.CompletedTask;
    }
}
