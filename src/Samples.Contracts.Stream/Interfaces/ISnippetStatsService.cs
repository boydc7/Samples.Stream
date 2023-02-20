using Samples.Contracts.Stream.Models;

namespace Samples.Contracts.Stream.Interfaces;

public interface ISnippetStatsService
{
    Task<IReadOnlyCollection<SnippetStat>> GetStatsAsync();
}
