namespace Samples.Contracts.Stream.Interfaces;

public interface IPartitionOffset
{
    public int Partition { get; set; }
    public long Offset { get; set; }
}
