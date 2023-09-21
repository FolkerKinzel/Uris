namespace FolkerKinzel.Uris.Intls;

internal static class Base64Parser
{
    internal static bool TryDecode(ReadOnlySpan<char> base64, [NotNullWhen(true)] out byte[]? decoded)
    {
        try
        {
            decoded = Base64.GetBytes(
                base64,
                Base64ParserOptions.AcceptMissingPadding | Base64ParserOptions.AcceptBase64Url);
            return true;
        }
        catch
        {
            decoded = null;
            return false;
        }
    }

}
