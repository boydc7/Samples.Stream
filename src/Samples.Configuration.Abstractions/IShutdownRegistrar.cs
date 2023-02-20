namespace Samples.Configuration.Abstractions;

public interface IShutdownRegistrar
{
    void Register(Action callback);
}
