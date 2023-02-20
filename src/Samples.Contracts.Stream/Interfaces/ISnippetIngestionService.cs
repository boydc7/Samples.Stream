using Samples.Contracts.Stream.Models;

namespace Samples.Contracts.Stream.Interfaces;

public interface ISnippetIngestionService
{
    ValueTask IngestAsync(Snippet snippet);
}
