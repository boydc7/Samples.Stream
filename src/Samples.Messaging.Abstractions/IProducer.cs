namespace Samples.Messaging.Abstractions;

public interface IProducer
{
    public int WorkerCount { get; }
    void Init();
    void OnShutdown();
}

public interface IProducer<in T> : IProducer
{
    ValueTask PublishAsync(T entity);
}

public interface ILocalProducer : IProducer { }
