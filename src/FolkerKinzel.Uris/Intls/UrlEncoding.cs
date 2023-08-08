using System.Net;
using System.Text;

namespace FolkerKinzel.Uris.Intls;


internal static class UrlEncoding
{
    //[ExcludeFromCodeCoverage]
    //internal static bool TryEncode(string input, [NotNullWhen(true)] out string? output)
    //{
    //    Debug.Assert(input != null);
    //    try
    //    {
    //        output = Uri.EscapeDataString(input);
    //    }
    //    catch
    //    {
    //        output = null;
    //        return false;
    //    }
    //    return true;
    //}

    [ExcludeFromCodeCoverage]
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

    internal static bool TryEncode(string value, string charSet, [NotNullWhen(true)] out string? encoded)
    {
        Debug.Assert(value != null);

        try
        {
            Encoding enc = TextEncodingConverter.GetEncoding(charSet, throwOnInvalidWebName: true);
            encoded = EncodeBytes(enc.GetBytes(value));
            return true;
        }
        catch
        {
            encoded = null;
            return false;
        }
    }

    internal static string EncodeBytes(byte[] value)
    {
        Debug.Assert(value != null);
        byte[] encodedBytes = WebUtility.UrlEncodeToBytes(value, 0, value.Length);
        return Encoding.ASCII.GetString(encodedBytes);
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



    //internal static string UrlEncodeValueWithCharset(string value, string? charSet)
    //{
    //    Encoding encoding = TextEncodingConverter.GetEncoding(charSet);
    //    var bytes = encoding.GetBytes(value);

    //    StringBuilder sb = new StringBuilder(3);

    //    for (int i = 0; i < bytes.Length; i++)
    //    {
    //        sb.Append('%');
    //        sb.Append(bytes[i].ToString("X2"));
    //    }
    //    return sb.ToString();
    //}
}
