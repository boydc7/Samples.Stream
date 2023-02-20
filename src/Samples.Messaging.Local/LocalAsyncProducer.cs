using Microsoft.Extensions.Logging;
using Samples.Configuration;
using Samples.Configuration.Abstractions;
using Samples.Configuration.Exceptions;
using Samples.Extensions;
using Samples.Messaging.Abstractions;

namespace Samples.Messaging.Local;

public class LocalAsyncProducer<T> : IProducer<T>, IDisposable, ILocalProducer
    where T : class, IDeferred
{
    protected readonly ILogger<LocalAsyncProducer<T>> _log;
    protected readonly IBuffer<T> _buffer;

    private readonly Dictionary<string, Task> _workers;
    private readonly object _lockObject = new();
    private readonly Func<string, IBuffer<T>, IConsumer> _workerFactory;
    private bool _inShutdown;
    private int _disposalCount;

    public LocalAsyncProducer(ILocalBuffer<T> buffer,
                              ILogger<LocalAsyncProducer<T>> log,
                              IShutdownRegistrar shutdownRegistrar,
                              int maxWorkers = 9,
                              Func<string, IBuffer<T>, IConsumer> workerFactory = null)
    {
        MaxWorkers = maxWorkers.Gz(11);

        _buffer = buffer;
        _log = log;

        _workerFactory = workerFactory ?? ((i, b) => new LocalAsyncWorker<T>(i, b));

        _workers = new Dictionary<string, Task>(MaxWorkers);

        shutdownRegistrar.Register(OnShutdown);
    }

    public async ValueTask PublishAsync(T entity)
    {
        if (_inShutdown)
        {
            throw new ApplicationInShutdownException();
        }

        await _buffer.AddAsync(entity);

        StartWorker();
    }

    public void Init()
    {
        if (_inShutdown || _workers.Count > 0)
        {
            return;
        }

        StartWorker();
    }

    public int MaxWorkers { get; }
    public int WorkerCount => _workers.Count;

    private void StartWorker()
    {
        if (_workers.Count >= MaxWorkers || _inShutdown)
        {
            return;
        }

        var consumerId = Guid.NewGuid().ToString("N");
        IConsumer consumer = null;

        try
        { // We got work to do
            lock(_lockObject)
            {
                if (_workers.Count >= MaxWorkers)
                {
                    return;
                }

                consumer = _workerFactory(consumerId, _buffer);

                _workers.Add(consumerId,
                             Task.Run(async () => await consumer.ReceiveAsync(), ApplicationShutdownCancellationSource.Instance.Token)
                                 .ContinueWith(_ => OnWorkerComplete(consumer)));
            }
        }
        catch(Exception ex)
        {
            if (consumer != null)
            {
                consumer.Exception ??= ex;

                OnWorkerComplete(consumer);
            }

            if (_workers.ContainsKey(consumerId))
            {
                lock(_lockObject)
                {
                    try
                    {
                        _workers.Remove(consumerId);
                    }
                    catch(Exception x)
                    {
                        _log.LogWarning(x, "Worker removal failed");
                    }
                }
            }
        }
    }

    private void OnWorkerComplete(IConsumer taskConsumer)
    {
        if (taskConsumer == null || taskConsumer.ConsumerId.IsNullOrEmpty())
        {
            return;
        }

        var consumerErrorMsg = taskConsumer.ErrorMessage;

        if (!consumerErrorMsg.IsNullOrEmpty())
        {
            _log.LogWarning(taskConsumer.Exception, "Consumer [{LocalConsumerId}] error [{ErrorMsg}]", taskConsumer.ConsumerId, consumerErrorMsg);
        }

        if (_workers.ContainsKey(taskConsumer.ConsumerId))
        {
            lock(_lockObject)
            {
                try
                {
                    _workers.Remove(taskConsumer.ConsumerId);
                }
                catch(Exception x)
                {
                    _log.LogWarning(x, "Worker removal failed");
                }
            }
        }
    }

    public void OnShutdown()
    {
        _inShutdown = true;

        _buffer.InShutdown = true;

        Dispose();
    }

    public virtual void Dispose()
    {
        if (Interlocked.CompareExchange(ref _disposalCount, 1, 0) > 0)
        {
            return;
        }

        if (_workers.Count > 0)
        {
            Task.WaitAll(_workers.Values.ToArray(), 25000);
        }

        Disposed = true;
    }

    public bool Disposed { get; private set; }
}
