namespace Samples.Messaging.Abstractions;

public interface IDeferred
{
    Task ExecuteAsync();
    int Attempts { get; }
    int MaxAttempts { get; set; }
    Action<object, Exception> OnError { get; set; }
}
