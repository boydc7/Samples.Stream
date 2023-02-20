namespace Samples.Contracts.Stream.Interfaces;

public interface ITransform<T1, T2>
{
    T2 Transform(T1 source);
    T1 Transform(T2 source);
}
