using Samples.Contracts.Stream.Models;

namespace Samples.Contracts.Stream.Interfaces;

public interface ISnippetProcessingService
{
    Task ProcessAsync(Snippet snippet);
}
