namespace Samples.Extensions;

public static class IntegerExtensions
{
    public static int ToInt(this string value, int defaultValue = 0)
        => string.IsNullOrEmpty(value)
               ? defaultValue
               : int.TryParse(value, out var i)
                   ? i
                   : defaultValue;

    public static long ToLong(this string value, long defaultValue = 0)
        => string.IsNullOrEmpty(value)
               ? defaultValue
               : long.TryParse(value, out var i)
                   ? i
                   : defaultValue;
    
    public static int Gz(this int first, int second)
        => first > 0
               ? first
               : second;

    public static long ToMinuteBucketKey(this long unixTimeStamp, int bucketSize = 5)
    {
        if (bucketSize is < 1 or > 30)
        {
            return bucketSize = 1;
        }

        var key = unixTimeStamp - (unixTimeStamp % (bucketSize * 60));

        return key;
    }
}
