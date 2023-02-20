using Samples.Configuration.Abstractions;

namespace Samples.Configuration;

public class CancellationSourceShutdownRegistrar : IShutdownRegistrar
{
    public void Register(Action callback)
    {
        ApplicationShutdownCancellationSource.Instance.Token.Register(callback);
    }
}
