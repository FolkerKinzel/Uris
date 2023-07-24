﻿namespace FolkerKinzel.Uris.Intls;

internal static class Base64Parser
{
    /// <summary>
    /// Parses a Base64 <see cref="string"/> as byte array - even
    /// if it is in the Base64Url format (RFC 4648 § 5).
    /// </summary>
    /// <param name="base64">A Base64 or Base64Url <see cref="string"/>.</param>
    /// <returns>A byte array containing the data that was encoded in <paramref name="base64"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="base64"/> is <c>null</c>.</exception>
    /// <exception cref="FormatException"><paramref name="base64"/> has an invalid format.</exception>
    internal static byte[] Parse(string base64)
    {
        if (base64 == null)
        {
            throw new ArgumentNullException(nameof(base64));
        }

        base64 = ConvertBase64UrlToBase64(base64);
        base64 = HandleBase64WithoutPadding(base64);

        return Convert.FromBase64String(base64);
    }

    private static string ConvertBase64UrlToBase64(string base64)
    {
        base64 = base64.Replace('-', '+');
        return base64.Replace('_', '/');
    }

    private static string HandleBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return base64;
    }

}
