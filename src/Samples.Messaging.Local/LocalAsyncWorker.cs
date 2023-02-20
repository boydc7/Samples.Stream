using Samples.Extensions;
using Samples.Messaging.Abstractions;

namespace Samples.Messaging.Local;

internal class LocalAsyncWorker<T> : IConsumer
    where T : class, IDeferred
{
    private readonly IBuffer<T> _buffer;
    private readonly int _maxAttemptsPer;
    private readonly TimeSpan _yieldTimeout;

    public LocalAsyncWorker(string consumerId, IBuffer<T> buffer,
                            int maxAttemptsPer = 5, TimeSpan? yieldTimeout = null)
    {
        ConsumerId = consumerId ?? Guid.NewGuid().ToString("N");
        _buffer = buffer;
        _maxAttemptsPer = maxAttemptsPer;
        _yieldTimeout = yieldTimeout ?? TimeSpan.FromSeconds(5);
    }

    public string ConsumerId { get; }

    public async Task ReceiveAsync()
    {
        try
        {
            await ProcessAsync();
        }
        catch(Exception ex)
        {
            Exception = ex;
        }
    }

    public Exception Exception { get; set; }

    public string ErrorMessage
    {
        get
        {
            if (Exception == null)
            {
                return string.Empty;
            }

            return string.Concat(Exception.Message,
                                 Exception.InnerException == null
                                     ? string.Empty
                                     : "\n Inner Exception: \n",
                                 Exception.InnerException?.Message ?? string.Empty);
        }
    }

    public bool YieldedToRecycle { get; private set; }

    private async Task ProcessAsync()
    {
        var lastReceived = DateTime.UtcNow;
        var startedProcessingAt = lastReceived;
        var myExceptionCount = 0;

        while (true)
        {
            T entity = null;

            try
            {
                entity = await _buffer.TakeAsync();
            }
            catch(Exception x)
            {
                myExceptionCount++;
                Exception = x;
            }

            if (entity == null)
            {
                if (_buffer.InShutdown || (DateTime.UtcNow - lastReceived) >= _yieldTimeout)
                {
                    return;
                }

                await Task.Delay(100);

                continue;
            }

            lastReceived = DateTime.UtcNow;

            try
            {
                await entity.ExecuteAsync();

                await _buffer.AckAsync(entity);

                Exception = null;
            }
            catch(Exception ex)
            {
                myExceptionCount++;

                await _buffer.NakAsync(entity);

                if (entity.Attempts < entity.MaxAttempts.Gz(_maxAttemptsPer))
                {
                    await Task.Delay(Math.Min(entity.Attempts, 20) * 250);
                }
                else
                {
                    Exception = ex;

                    entity.OnError?.Invoke(entity, ex);
                }
            }
        }
    }
}
