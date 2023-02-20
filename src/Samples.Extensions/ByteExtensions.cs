using System.Security.Cryptography;

namespace Samples.Extensions;

public static class ByteExtensions
{
    public static byte[] ToSha256(this byte[] bytes)
    {
        if (bytes == null || !bytes.Any())
        {
            return null;
        }

        var ha = SHA256.Create();

        if (ha == null)
        {
            return null;
        }

        var hashValue = ha.ComputeHash(bytes);
        ha.Clear();

        return hashValue;
    }

    public static string ToSha256Base64(this byte[] bytes)
    {
        var hash = bytes.ToSha256();

        if (hash == null || !hash.Any())
        {
            return null;
        }

        return ToBase64(hash);
    }

    public static string ToBase64(this byte[] bytes) => Convert.ToBase64String(bytes);
}
