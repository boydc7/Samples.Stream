namespace Samples.Configuration;

public class ApplicationShutdownCancellationSource
{
    private ApplicationShutdownCancellationSource() { }

    public static readonly ApplicationShutdownCancellationSource Instance = new();

    public void TryCancel()
    {
        try
        {
            CancellationTokenSource?.Cancel();
        }
        catch
        {
            // Ignore - shuting down
        }
    }

    public CancellationTokenSource CancellationTokenSource { get; } = new();
    public CancellationToken Token => CancellationTokenSource.Token;
}
