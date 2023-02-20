namespace Samples.Messaging.Abstractions;

public interface ILocalDeferrer
{
    void Defer<T>(T obj, Func<T, Task> callbackAsync, bool force = false, Action<T, Exception> onError = null, int maxAttempts = 0)
        where T : class;

    ValueTask DeferAsync<T>(T obj, Func<T, Task> callbackAsync, bool force = false,
                            Action<T, Exception> onError = null, int maxAttempts = 0)
        where T : class;
}

public interface ILocalDeferrerObserver
{
    void OnDeferred();
}
