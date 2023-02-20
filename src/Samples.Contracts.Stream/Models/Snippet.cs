namespace Samples.Contracts.Stream.Models;

public record Snippet
{
    public string Body { get; set; }
    public string UserId { get; set; }
    public long CreatedOnUtc { get; set; }
    public IReadOnlySet<string> Tags { get; set; }
    public IReadOnlySet<string> Mentions { get; set; }
}
