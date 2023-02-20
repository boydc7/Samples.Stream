namespace Samples.Messaging.Abstractions;

public interface ILocalBuffer<T> : IBuffer<T>
{
    void Add(T item);
    int Count { get; }
}

public interface IBuffer<T>
{
    ValueTask AddAsync(T item);
    ValueTask AckAsync(T item);
    ValueTask NakAsync(T item);
    ValueTask<T> TakeAsync();
    bool InShutdown { get; set; }
}
