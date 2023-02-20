namespace Samples.Contracts.Stream.Models;

public record SnippetStat
{
    public string KeyName { get; set; }
    public string StatName { get; set; }
    public double StatValue { get; set; }
}
