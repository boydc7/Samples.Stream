using Samples.Contracts.Stream.Interfaces;

namespace Samples.Contracts.Stream.Models;

public record ConsumedEvent<T> : IPartitionOffset
{
    public int Partition { get; set; }
    public long Offset { get; set; }
    public T Event { get; set; }
}
