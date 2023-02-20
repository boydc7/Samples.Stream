using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Samples.Messaging.Abstractions;

namespace Samples.Messaging.Local;

public class LocalDeferred : IDeferred
{
    public string TaskId { get; set; } = Guid.NewGuid().ToString("N");
    public Func<object, Task> Callback { get; set; }

    public object ObjectRef { get; set; }
    public int Attempts { get; private set; }
    public int MaxAttempts { get; set; }

    [IgnoreDataMember]
    [JsonIgnore]
    public Action<object, Exception> OnError { get; set; }

    public Task ExecuteAsync()
    {
        Attempts++;

        return Callback == null
                   ? Task.CompletedTask
                   : Callback(ObjectRef);
    }

    private string _toString;

    public override string ToString() => _toString ??= string.Concat("TaskId [", TaskId, "] - [", ObjectRef?.GetType().Name ?? "NULL", "].[", Callback?.Method.Name ?? "UNKNOWN");
}
