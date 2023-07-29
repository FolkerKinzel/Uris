namespace FolkerKinzel.Uris.Intls;

/// <summary>
/// Provides functionality to build a "data" URL (RFC 2397) that 
/// embeds data in-line in a URL. A "data" URL can be created automatically from 
/// the data to embed. This can be a file, a byte array or a <see cref="string"/>. 
/// </summary>
internal static class DataUrlBuilder
{
    internal const int ESTIMATED_MIME_TYPE_LENGTH = 80;
    internal const string UTF_8 = "utf-8";
    private const string CHARSET_KEY = "charset";
    private const int COMMA_LENGTH = 1;

    /// <summary>
    /// Appends embedded text as "data" URL (RFC 2397) to the end of a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> to which a "data" URL is appended.</param>
    /// <param name="text">The text to embed into the "data" URL. <paramref name="text"/> MUST not be URL encoded.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the <paramref name="text"/>.</param>
    /// 
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// <exception cref="FormatException"><paramref name="text"/> could not URL-encoded.</exception>
    internal static StringBuilder AppendEmbeddedTextInternal(this StringBuilder builder, string? text, MimeType mimeType)
    {
        Debug.Assert(builder != null);
        Debug.Assert(mimeType != null);

        text ??= string.Empty;

        string data = Uri.EscapeDataString(text);

        if (data.Length == text.Length)
        {
            mimeType.RemoveParameter(CHARSET_KEY);
        }
        else
        {
            mimeType.AppendParameter(CHARSET_KEY, UTF_8);
        }

        _ = builder.EnsureCapacity(builder.Length
                                   + DataUrl.Protocol.Length
                                   + ESTIMATED_MIME_TYPE_LENGTH
                                   + COMMA_LENGTH
                                   + data.Length);
        return builder.Append(DataUrl.Protocol).AppendMediaType(mimeType).Append(',').Append(data);

        // $"data:,{Uri.EscapeDataString(text)}"
    }

    /// <summary>
    /// Appends binary data as "data" URL (RFC 2397) to the end of a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> to which a "data" URL is appended.</param>
    /// <param name="bytes">The binary data to embed into the "data" URL.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the <paramref name="bytes"/>.</param>
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    internal static StringBuilder AppendEmbeddedBytesInternal(this StringBuilder builder, byte[]? bytes, MimeType mimeType)
    {
        Debug.Assert(builder != null);
        Debug.Assert(mimeType != null);

        string data = bytes is null ? string.Empty : Convert.ToBase64String(bytes, Base64FormattingOptions.None);
        _ = builder.EnsureCapacity(builder.Length
                                   + DataUrl.Protocol.Length
                                   + ESTIMATED_MIME_TYPE_LENGTH
                                   + DataUrl.Base64.Length
                                   + COMMA_LENGTH
                                   + data.Length);
        return builder.Append(DataUrl.Protocol).AppendMediaType(mimeType).Append(DataUrl.Base64).Append(',').Append(data);

        // $"data:{mediaTypeString};base64,{Convert.ToBase64String(bytes)}"
    }

    /// <summary>
    /// Appends the content of a file as "data" URL (RFC 2397) to the end of a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> to which a "data" URL is appended.</param>
    /// <param name="filePath">Abolute path to the file which content is to embed into the "data" URL.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the file whose content is to embed.</param>
    /// 
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// 
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is not a valid file path.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    internal static StringBuilder AppendFileContentInternal(this StringBuilder builder, string filePath, MimeType mimeType)
    {
        Debug.Assert(builder != null);
        Debug.Assert(filePath != null);
        Debug.Assert(mimeType != null);

        return builder.AppendEmbeddedBytesInternal(LoadFile(filePath), mimeType);
    }


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

}
