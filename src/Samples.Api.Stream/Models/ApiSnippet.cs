using System.ComponentModel.DataAnnotations;

namespace Samples.Api.Stream.Models;

public record ApiSnippet
{
    [Required]
    [MinLength(1)]
    [MaxLength(5000)]
    public string Body { get; set; }
    
    [Required]
    [MinLength(1)]
    public string UserId { get; set; }
    
    public long CreatedOnUtc { get; set; }
    public IReadOnlyCollection<string> Tags { get; set; }
    public IReadOnlyCollection<string> Mentions { get; set; }
}
