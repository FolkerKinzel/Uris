using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris;

/// <summary>
/// Provides functionality to build a "data" URL (RFC 2397) that 
/// embeds data in-line in a URL. A "data" URL can be created automatically from 
/// the data to embed. This can be a file, a byte array or a <see cref="string"/>. 
/// </summary>
/// <example>
/// <note type="note">
/// For the sake of better readability, exception handling is ommitted in the example.
/// </note>
/// <para>
/// Creating and parsing a "data" URL:
/// </para>
/// <code language="c#" source="./../Examples/DataUrlExample.cs"/>
/// </example>
public static class DataUrlBuilder
{
    #region const
    internal const string Protocol = "data:";
    internal const string Base64 = ";base64";
    internal const string DEFAULT_MEDIA_TYPE = "text/plain";
    private const int ESTIMATED_MIME_TYPE_LENGTH = 80;
    #endregion

    /// <summary>
    /// Embeds text in a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="text">The text to embed into the "data" URL. <paramref name="text"/> MUST not be URL encoded.</param>
    /// <param name="mimeTypeString">The Internet Media Type of the <paramref name="text"/> or <c>null</c> for <see cref="MimeType.Default"/>.</param>
    /// 
    /// <returns>A "data" URL, into which <paramref name="text"/> is embedded.</returns>
    /// <exception cref="FormatException"><paramref name="text"/> consists only of ASCII characters and the <see cref="Uri"/> class 
    /// was not able to encode <paramref name="text"/> correctly.</exception>
    /// <remarks>If <paramref name="text"/>
    /// consists only of ASCII characters the method serializes it Url encoded, otherwise <paramref name="text"/> is serialized
    /// as Base64 using <see cref="UTF8Encoding"/>.</remarks>
    public static string FromText(string? text, string? mimeTypeString = DEFAULT_MEDIA_TYPE) =>
        MimeType.TryParse(string.IsNullOrWhiteSpace(mimeTypeString) ? MimeType.Default : mimeTypeString, out MimeType mimeType)
                ? FromText(text, mimeType)
                : FromText(text, MimeType.Default);


    /// <summary>
    /// Embeds text in a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="text">The text to embed into the "data" URL. <paramref name="text"/> MUST not be URL encoded.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the <paramref name="text"/>.</param>
    /// 
    /// <returns>A "data" URL, into which <paramref name="text"/> is embedded.</returns>
    /// <exception cref="FormatException"><paramref name="text"/> consists only of ASCII characters and the <see cref="Uri"/> class 
    /// was not able to encode <paramref name="text"/> correctly.</exception>
    /// <remarks>If <paramref name="text"/>
    /// consists only of ASCII characters the method serializes it Url encoded, otherwise <paramref name="text"/> is serialized
    /// as Base64 using <see cref="UTF8Encoding"/>.</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="mimeType"/> is <c>null</c>.</exception>
    public static string FromText(string? text, MimeType mimeType)
    {
        if (mimeType is null)
        {
            throw new ArgumentNullException(nameof(mimeType));
        }

        text ??= string.Empty;
        text = Uri.UnescapeDataString(text);

        if (text.IsAscii())
        {
            string data = Uri.EscapeDataString(text);
            var sb = new StringBuilder(Protocol.Length + ESTIMATED_MIME_TYPE_LENGTH + data.Length);
            return sb.Append(Protocol).AppendMediaType(mimeType).Append(',').Append(data).ToString();
        }
        else
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            mimeType.AppendParameter("charset", "utf-8");
            return FromBytes(bytes, mimeType);
        }

        // $"data:,{Uri.EscapeDataString(text)}"
    }


    /// <summary>
    /// Embeds binary data in a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="bytes">The binary data to embed into the "data" URL.</param>
    /// <param name="mimeTypeString">The Internet Media Type of the <paramref name="bytes"/> or <c>null</c> for <see cref="MimeType.Default"/>.</param>
    /// <returns>A "data" URL, into which the binary data provided by the parameter <paramref name="bytes"/> is embedded.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="mimeTypeString"/> is <c>null</c>.</exception>
    public static string FromBytes(byte[]? bytes, string? mimeTypeString = MimeType.Default) =>
        MimeType.TryParse(string.IsNullOrWhiteSpace(mimeTypeString) ? MimeType.Default : mimeTypeString, out MimeType mimeType)
                ? FromBytes(bytes, mimeType)
                : FromBytes(bytes, MimeType.Default);


    /// <summary>
    /// Embeds binary data in a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="bytes">The binary data to embed into the "data" URL.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the <paramref name="bytes"/>.</param>
    /// <returns>A "data" URL, into which the binary data provided by the parameter <paramref name="bytes"/> is embedded.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="mimeType"/> is <c>null</c>.</exception>
    public static string FromBytes(byte[]? bytes, MimeType mimeType)
    {

        if (mimeType is null)
        {
            throw new ArgumentNullException(nameof(mimeType));
        }

        string data = bytes is null ? string.Empty : Convert.ToBase64String(bytes, Base64FormattingOptions.None);
        var builder = new StringBuilder(Protocol.Length + ESTIMATED_MIME_TYPE_LENGTH + Base64.Length + 1 + data.Length);
        return builder.Append(Protocol).AppendMediaType(mimeType).Append(Base64).Append(',').Append(data).ToString();

        // $"data:{mediaTypeString};base64,{Convert.ToBase64String(bytes)}"
    }


    /// <summary>
    /// Embeds the content of a file in a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="filePath">Abolute path to the file which content is to embed into the "data" URL.</param>
    /// <param name="mimeTypeString">The Internet Media Type ("MIME type") of the file to embed or <c>null</c> to let the method automatically
    /// retrieve the <see cref="MimeType"/> from the file type extension.</param>
    /// <returns>A "data" URL, into which the content of the file provided by the parameter <paramref name="filePath"/> is embedded.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is not a valid file path.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    /// 
    ///<example>
    /// <note type="note">
    /// For the sake of better readability, exception handling is ommitted in the example.
    /// </note>
    /// <para>
    /// Creating and parsing a "data" URL:
    /// </para>
    /// <code language="c#" source="./../Examples/DataUrlExample.cs"/>
    /// </example>
    public static string FromFile(string filePath, string? mimeTypeString = null) =>
        MimeType.TryParse(string.IsNullOrWhiteSpace(mimeTypeString) ? MimeType.Default : mimeTypeString, out MimeType mimeType)
                ? FromFile(filePath, mimeType)
                : FromFile(filePath, MimeType.Default);

    /// <summary>
    /// Embeds the content of a file in a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="filePath">Abolute path to the file which content is to embed into the "data" URL.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the file to embed.</param>
    /// 
    /// <returns>A "data" URL, into which the content of the file provided by the parameter <paramref name="filePath"/> is embedded.</returns>
    /// 
    ///<example>
    /// <note type="note">
    /// For the sake of better readability, exception handling is ommitted in the example.
    /// </note>
    /// <para>
    /// Creating and parsing a "data" URL:
    /// </para>
    /// <code language="c#" source="./../Examples/DataUrlExample.cs"/>
    /// </example>
    /// 
    ///<exception cref="ArgumentNullException"><paramref name="filePath"/> or <paramref name="mimeType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is not a valid file path.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static string FromFile(string filePath, MimeType mimeType)
    {
        if (mimeType is null)
        {
            throw new ArgumentNullException(nameof(mimeType));
        }

        byte[] bytes = LoadFile(filePath);

        return FromBytes(bytes, mimeType);
    }

    #region private

    [ExcludeFromCodeCoverage]
    private static byte[] LoadFile(string path)
    {
        try
        {
            return File.ReadAllBytes(path);
        }
        catch (ArgumentNullException)
        {
            throw new ArgumentNullException(nameof(path));
        }
        catch (ArgumentException e)
        {
            throw new ArgumentException(e.Message, nameof(path), e);
        }
        catch (UnauthorizedAccessException e)
        {
            throw new IOException(e.Message, e);
        }
        catch (NotSupportedException e)
        {
            throw new ArgumentException(e.Message, nameof(path), e);
        }
        catch (System.Security.SecurityException e)
        {
            throw new IOException(e.Message, e);
        }
        catch (PathTooLongException e)
        {
            throw new ArgumentException(e.Message, nameof(path), e);
        }
        catch (Exception e)
        {
            throw new IOException(e.Message, e);
        }
    }

    #endregion


}
