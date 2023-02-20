namespace Samples.Api.Stream.Models;

public record ApiSnippetStat
{
    public string KeyName { get; set; }
    public IReadOnlyList<ApiSnippetStatValue> Stats { get; set; }

}

public record ApiSnippetStatValue
{
    public string StatName { get; set; }
    public double StatValue { get; set; }
}
