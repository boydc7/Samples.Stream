using Samples.Contracts.Stream.Models;

namespace Samples.Contracts.Stream.Interfaces;

public interface ISnippetReader
{
    IEnumerable<ConsumedEvent<Snippet>> Read();

    void AckConsume(int partition, long offset);
    void Stop();
}
