namespace Samples.Extensions.Hosting.Health;

public class SampleApplicationShutdownCancellationSource
{
    private SampleApplicationShutdownCancellationSource() { }

    public static readonly SampleApplicationShutdownCancellationSource Instance = new();

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
