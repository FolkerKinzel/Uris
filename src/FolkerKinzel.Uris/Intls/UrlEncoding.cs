using System.Net;
using System.Text;

namespace FolkerKinzel.Uris.Intls;

internal static class UrlEncoding
{
    internal static bool TryDecode(string value, string charSet, [NotNullWhen(true)] out string? decoded)
    {
        try
        {
            decoded = UnescapeValueFromUrlEncoding(value, charSet);
            return true;
        }
        catch
        {
            decoded = null;
            return false;
        }
    }


    internal static string EncodeBytes(byte[] value)
    {
        Debug.Assert(value != null);
        byte[] encodedBytes = WebUtility.UrlEncodeToBytes(value, 0, value.Length);

        // WebUtility guaranties UTF-8 not ASCII
        return Encoding.UTF8.GetString(encodedBytes);
    }


    internal static bool TryDecodeBytes(ReadOnlySpan<char> value, [NotNullWhen(true)] out byte[]? decoded)
    {
        try
        {
            Encoding ascii = TextEncoding.InitThrowingEncoding(20127);
            byte[] bytes = ascii.GetBytes(value.ToString());
            decoded = WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length);
            return true;
        }
        catch
        {
            decoded = null;
            return false;
        }
    }

    /// <summary>
    /// Removes URL encoding from <paramref name="value"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="charSet"></param>
    /// <returns></returns>
    /// <exception cref="DecoderFallbackException"></exception>
    /// <exception cref="EncoderFallbackException"></exception>
    private static string UnescapeValueFromUrlEncoding(string value, string charSet)
    {
        string result;

        Encoding encoding = TextEncoding.InitThrowingEncoding(charSet);
        Encoding ascii = TextEncoding.InitThrowingEncoding(20127);

        byte[] bytes = ascii.GetBytes(value);

        result = encoding.GetString(WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length));
        return result;
    }

}
