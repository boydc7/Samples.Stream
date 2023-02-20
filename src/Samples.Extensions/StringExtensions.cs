using System.Globalization;

namespace Samples.Extensions;

public static class StringExtensions
{
    public static string ToStringInvariant<T>(this T source)
        where T : IFormattable
        => source == null
               ? null
               : source.ToString(null, CultureInfo.InvariantCulture);

    public static string LastRightPart(this string source, char indexOf)
    {
        if (string.IsNullOrEmpty(source))
        {
            return source;
        }

        var index = source.LastIndexOf(indexOf);

        return index < 0
                   ? source
                   : source[(index + 1)..];
    }

    public static string Coalesce(this string first, string second)
        => string.IsNullOrEmpty(first)
               ? second
               : first;

    public static bool IsNullOrEmpty(this string source)
        => string.IsNullOrEmpty(source);
}
