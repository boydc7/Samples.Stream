using System.Collections.Concurrent;
using Samples.Messaging.Abstractions;

namespace Samples.Messaging.Local;

internal class InMemoryBuffer<T> : ILocalBuffer<T>
    where T : IDeferred
{
    private int? _maxQueueSize;

    private readonly ConcurrentQueue<T> _buffer = new();

    private int MaxQueueSize => _maxQueueSize ?? (_maxQueueSize = 150000).Value;

    public int Count => _buffer.Count;

    public bool InShutdown { get; set; }

    public void Add(T item)
    {
        CheckSize();

        _buffer.Enqueue(item);
    }

    public ValueTask AddAsync(T item)
    {
        Add(item);

        return new ValueTask();
    }

    public ValueTask AckAsync(T item) => new();

    public ValueTask NakAsync(T item)
    {
        if (item != null && item.Attempts < item.MaxAttempts)
        {
            return AddAsync(item);
        }

        return new ValueTask();
    }

    public ValueTask<T> TakeAsync() => new(_buffer.TryDequeue(out var buffered)
                                               ? buffered
                                               : default);

    private void CheckSize()
    {
        while (_buffer.Count >= MaxQueueSize)
        {
            _buffer.TryDequeue(out _);
        }
    }
}
