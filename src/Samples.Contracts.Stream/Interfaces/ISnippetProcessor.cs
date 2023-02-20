using Samples.Contracts.Stream.Models;

namespace Samples.Contracts.Stream.Interfaces;

public interface ISnippetProcessor
{
    Task ProcessAsync(Snippet snippet);
    Task<ICollection<SnippetStat>> GetValuesAsync();
}
