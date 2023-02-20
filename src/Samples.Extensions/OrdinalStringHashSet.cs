namespace Samples.Extensions;

public class OrdinalStringHashSet : HashSet<string>
{
    public OrdinalStringHashSet()
        : base(StringComparer.OrdinalIgnoreCase) { }

    public OrdinalStringHashSet(IEnumerable<string> collection)
        : base(collection ?? Enumerable.Empty<string>(), StringComparer.OrdinalIgnoreCase) { }
}

public static class OrdinalStringHashSetHelpers
{
    public static OrdinalStringHashSet ToOrdinalStringHashSet(this IEnumerable<string> source)
        => ToOrdinalStringHashSet(source, v => v);

    public static OrdinalStringHashSet ToOrdinalStringHashSet<T>(this IEnumerable<T> source, Func<T, string> valSelector)
    {
        var hashSet = new OrdinalStringHashSet();

        if (source == null)
        {
            return hashSet;
        }

        foreach (var sourceValue in source)
        {
            hashSet.Add(valSelector(sourceValue));
        }

        return hashSet;
    }
}
