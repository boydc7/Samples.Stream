namespace Samples.Messaging.Abstractions;

public interface IConsumer
{
    Task ReceiveAsync();
    string ConsumerId { get; }
    Exception Exception { get; set; }
    string ErrorMessage { get; }
}
