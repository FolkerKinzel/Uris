using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris;

/// <summary>
/// Provides methods to handle <see cref="string"/>s that represent "data" URLs (RFC 2397).
/// </summary>
public static class DataUrl
{
    /// <summary>
    /// The scheme that indicates a "data" Url (RFC 2397).
    /// </summary>
    public const string Scheme = "data:";

    

    /// <summary>
    /// The default Internet Media Type for a "data" Url (RFC 2397) if no other is specified.
    /// </summary>
    public const string DefaultMediaType = "text/plain";

    internal const string Base64 = ";base64";

    /// <summary>
    /// Embeds text URL-encoded into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="text">The text to embed into the "data" URL. <paramref name="text"/> MUST not be passed 
    /// URL-encoded.</param>
    /// <param name="mimeTypeString">The Internet Media Type of the <paramref name="text"/> or <c>null</c> for 
    /// <see cref="DataUrl.DefaultMediaType"/>.</param>
    /// <param name="dataEncoding">The encoding to use to embed the <paramref name="text"/>.</param>
    /// 
    /// <returns>A "data" URL, into which <paramref name="text"/> is embedded.</returns>
    /// 
    /// <exception cref="FormatException"><paramref name="text"/> could not URL-encoded.</exception>
    [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
    public static string FromText(string? text,
                                  string? mimeTypeString = DataUrl.DefaultMediaType,
                                  DataEncoding dataEncoding = DataEncoding.Url) =>
        AppendEmbeddedTextTo(new StringBuilder(), text, mimeTypeString, dataEncoding).ToString();


    /// <summary>
    /// Embeds text URL-encoded into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="text">The text to embed into the "data" URL. <paramref name="text"/> MUST not 
    /// be passed URL-encoded.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the <paramref name="text"/>.</param>
    /// <param name="dataEncoding">The encoding to use to embed the <paramref name="text"/>.</param>
    /// 
    /// <returns>A "data" URL, into which <paramref name="text"/> is embedded.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="mimeType"/> is <c>null</c>.</exception>
    /// <exception cref="FormatException"><paramref name="text"/> could not URL-encoded.</exception>
    public static string FromText(string? text, MimeType mimeType, DataEncoding dataEncoding = DataEncoding.Url) => 
        AppendEmbeddedTextTo(new StringBuilder(), text, mimeType, dataEncoding).ToString();


    /// <summary>
    /// Embeds binary data into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="bytes">The binary data to embed into the "data" URL.</param>
    /// <param name="mimeTypeString">The Internet Media Type of the <paramref name="bytes"/> or <c>null</c> for 
    /// <see cref="MimeString.OctetStream"/>.</param>
    /// <param name="dataEncoding">The encoding to use to embed the <paramref name="bytes"/>.</param>
    /// 
    /// <returns>A "data" URL, into which the binary data provided by the parameter <paramref name="bytes"/> is embedded.</returns>
    [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
    public static string FromBytes(byte[]? bytes,
                                   string? mimeTypeString = MimeString.OctetStream,
                                   DataEncoding dataEncoding = DataEncoding.Base64) =>
        AppendEmbeddedBytesTo(new StringBuilder(), bytes, mimeTypeString, dataEncoding).ToString();


    /// <summary>
    /// Embeds binary data into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="bytes">The binary data to embed into the "data" URL.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the <paramref name="bytes"/>.</param>
    /// <param name="dataEncoding">The encoding to use to embed the <paramref name="bytes"/>.</param>
    /// 
    /// <returns>A "data" URL, into which the binary data provided by the parameter <paramref name="bytes"/> is embedded.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="mimeType"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FromBytes(byte[]? bytes,
                                   MimeType mimeType,
                                   DataEncoding dataEncoding = DataEncoding.Base64) => 
        AppendEmbeddedBytesTo(new StringBuilder(), bytes, mimeType, dataEncoding).ToString();



    /// <summary>
    /// Embeds the content of a file into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="filePath">Path to the file whose content is to embed into the "data" URL.</param>
    /// <param name="mimeTypeString">The Internet Media Type ("MIME type") of the file whose content is to embed or <c>null</c> 
    /// to let the method automatically
    /// retrieve the <see cref="MimeType"/> from the file type extension.</param>
    /// <param name="dataEncoding">The encoding to use to embed the file content.</param>
    /// 
    /// <returns>A "data" URL, into which the content of the file provided by the parameter 
    /// <paramref name="filePath"/> is embedded.</returns>
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
    public static string FromFile(string filePath,
                                  string? mimeTypeString = null,
                                  DataEncoding dataEncoding = DataEncoding.Base64) =>
        AppendEmbeddedFileTo(new StringBuilder(), filePath, mimeTypeString, dataEncoding).ToString();


    /// <summary>
    /// Embeds the content of a file into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="filePath">Path to the file whose content is to embed into the "data" URL.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the file whose content is to embed.</param>
    /// <param name="dataEncoding">The encoding to use to embed the file content.</param>
    /// 
    /// <returns>A "data" URL into which the content of the file provided by the parameter <paramref name="filePath"/> is embedded.</returns>
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
    public static string FromFile(string filePath,
                                  MimeType mimeType,
                                  DataEncoding dataEncoding = DataEncoding.Base64) => 
        AppendEmbeddedFileTo(new StringBuilder(), filePath, mimeType, dataEncoding).ToString();


    /// <summary>
    /// Appends embedded text as "data" URL (RFC 2397) to the end of a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> to which a "data" URL is appended.</param>
    /// <param name="text">The text to embed into the "data" URL. <paramref name="text"/> MUST not be 
    /// passed URL-encoded.</param>
    /// <param name="mimeTypeString">The Internet Media Type of the <paramref name="text"/> or <c>null</c> for 
    /// <see cref="DataUrl.DefaultMediaType"/>.</param>
    /// <param name="dataEncoding">The encoding to use to embed the <paramref name="text"/>.</param>
    /// 
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="builder"/>
    /// is <c>null</c>.</exception>
    /// <exception cref="FormatException"><paramref name="text"/> could not URL-encoded.</exception>
    public static StringBuilder AppendEmbeddedTextTo(StringBuilder builder,
                                                     string? text,
                                                     string? mimeTypeString = DataUrl.DefaultMediaType,
                                                     DataEncoding dataEncoding = DataEncoding.Url) =>
         MimeType.TryParse(string.IsNullOrWhiteSpace(mimeTypeString) ? DataUrl.DefaultMediaType : mimeTypeString, out MimeType? mimeType)
                ? AppendEmbeddedTextTo(builder, text, mimeType, dataEncoding)
                : AppendEmbeddedTextTo(builder, text, DataUrl.DefaultMediaType, dataEncoding);


    /// <summary>
    /// Appends embedded text as "data" URL (RFC 2397) to the end of a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> to which a "data" URL is appended.</param>
    /// <param name="text">The text to embed into the "data" URL. <paramref name="text"/> MUST not be 
    /// passed URL-encoded.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the <paramref name="text"/>.</param>
    /// <param name="dataEncoding">The encoding to use to embed the <paramref name="text"/>.</param>
    /// 
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="mimeType"/> 
    /// is <c>null</c>.</exception>
    /// <exception cref="FormatException"><paramref name="text"/> could not URL-encoded.</exception>
    public static StringBuilder AppendEmbeddedTextTo(StringBuilder builder,
                                                     string? text,
                                                     MimeType mimeType,
                                                     DataEncoding dataEncoding = DataEncoding.Url) => 
        builder is null
            ? throw new ArgumentNullException(nameof(builder))
            : mimeType is null 
                ? throw new ArgumentNullException(nameof(mimeType))
                : builder.AppendEmbeddedTextInternal(text, mimeType, dataEncoding);


    /// <summary>
    /// Appends binary data as "data" URL (RFC 2397) to the end of a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> to which a "data" URL is appended.</param>
    /// <param name="bytes">The binary data to embed into the "data" URL.</param>
    /// <param name="mimeTypeString">The Internet Media Type of the <paramref name="bytes"/> or <c>null</c> for 
    /// <see cref="MimeString.OctetStream"/>.</param>
    /// <param name="dataEncoding">The encoding to use to embed the <paramref name="bytes"/>.</param>
    /// 
    /// 
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
    public static StringBuilder AppendEmbeddedBytesTo(StringBuilder builder,
                                                      byte[]? bytes,
                                                      string? mimeTypeString = MimeString.OctetStream,
                                                      DataEncoding dataEncoding = DataEncoding.Base64) =>
        MimeType.TryParse(string.IsNullOrWhiteSpace(mimeTypeString) ? MimeString.OctetStream : mimeTypeString, out MimeType? mimeType)
                ? AppendEmbeddedBytesTo(builder, bytes, mimeType, dataEncoding)
                : AppendEmbeddedBytesTo(builder, bytes, MimeString.OctetStream, dataEncoding);


    /// <summary>
    /// Appends binary data as "data" URL (RFC 2397) to the end of a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> to which a "data" URL is appended.</param>
    /// <param name="bytes">The binary data to embed into the "data" URL.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the <paramref name="bytes"/>.</param>
    /// <param name="dataEncoding">The encoding to use to embed the <paramref name="bytes"/>.</param>
    /// 
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="mimeType"/>
    /// is <c>null</c>.</exception>
    public static StringBuilder AppendEmbeddedBytesTo(StringBuilder builder,
                                                      byte[]? bytes,
                                                      MimeType mimeType,
                                                      DataEncoding dataEncoding = DataEncoding.Base64) =>
        builder is null
            ? throw new ArgumentNullException(nameof(builder))
            : mimeType is null
                ? throw new ArgumentNullException(nameof(mimeType))
                : builder.AppendEmbeddedBytesInternal(bytes, mimeType, dataEncoding);


    /// <summary>
    /// Appends the content of a file as "data" URL (RFC 2397) to the end of a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> to which a "data" URL is appended.</param>
    /// <param name="filePath">Abolute path to the file whose content is to embed into the "data" URL.</param>
    /// <param name="mimeTypeString">The Internet Media Type ("MIME type") of the file whose content is to embed or <c>null</c> 
    /// to let the method automatically
    /// retrieve the <see cref="MimeType"/> from the file type extension.</param>
    /// <param name="dataEncoding">The encoding to use to embed the file content.</param>
    /// 
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="filePath"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is not a valid file path.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static StringBuilder AppendEmbeddedFileTo(this StringBuilder builder,
                                                    string filePath,
                                                    string? mimeTypeString = null,
                                                    DataEncoding dataEncoding = DataEncoding.Base64) =>
        string.IsNullOrWhiteSpace(mimeTypeString)
              ? AppendEmbeddedFileTo(builder, filePath, MimeType.FromFileName(filePath), dataEncoding)
              : MimeType.TryParse(mimeTypeString, out MimeType? mimeType)
                  ? AppendEmbeddedFileTo(builder, filePath, mimeType, dataEncoding)
                  : AppendEmbeddedFileTo(builder, filePath, (string?)null, dataEncoding);



    /// <summary>
    /// Appends the content of a file as "data" URL (RFC 2397) to the end of a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="StringBuilder"/> to which a "data" URL is appended.</param>
    /// <param name="filePath">Abolute path to the file whose content is to embed into the "data" URL.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the file whose content is to embed.</param>
    /// <param name="dataEncoding">The encoding to use to embed the file content.</param>
    /// 
    /// <returns>A reference to <paramref name="builder"/>.</returns>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="builder"/>, <paramref name="filePath"/>, 
    /// or <paramref name="mimeType"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> is not a valid file path.</exception>
    /// <exception cref="IOException">I/O error.</exception>
    public static StringBuilder AppendEmbeddedFileTo(this StringBuilder builder,
                                                      string filePath,
                                                      MimeType mimeType,
                                                      DataEncoding dataEncoding = DataEncoding.Base64) => 
        builder is null
            ? throw new ArgumentNullException(nameof(builder))
            : filePath is null
                ? throw new ArgumentNullException(nameof(filePath))
                : mimeType is null 
                    ? throw new ArgumentNullException(nameof(mimeType))
                    : builder.AppendFileContentInternal(filePath, mimeType, dataEncoding);


    /// <summary>
    /// Tries to retrieve the embedded data from the <paramref name="dataUrl"/>.
    /// </summary>
    /// <param name="dataUrl">A "data" URL according to RFC 2397.</param>
    /// <param name="data">The embedded data. This can be either a <see cref="string"/> or a byte 
    /// array. The parameter is passed uninitialized.</param>
    /// <param name="fileTypeExtension">The file type extension for <paramref name="data"/>. The extension starts with the period ".".
    /// The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="dataUrl"/> is a valid "data" URL, otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetEmbeddedData(string? dataUrl,
                                  [NotNullWhen(true)] out object? data,
                                  [NotNullWhen(true)] out string? fileTypeExtension) =>
        TryGetEmbeddedData(dataUrl.AsMemory(), out data, out fileTypeExtension);


    /// <summary>
    /// Tries to retrieve the embedded data from the <paramref name="dataUrl"/>.
    /// </summary>
    /// <param name="dataUrl">A read only memory that contains a "data" URL according to RFC 2397.</param>
    /// <param name="data">The embedded data. This can be either a <see cref="string"/> or a byte array. 
    /// The parameter is passed uninitialized.</param>
    /// <param name="fileTypeExtension">The file type extension for <paramref name="data"/>.  The extension starts with the period ".". The parameter 
    /// is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="dataUrl"/> is a valid "data" URL, otherwise <c>false</c>.</returns>
    public static bool TryGetEmbeddedData(ReadOnlyMemory<char> dataUrl,
                                  [NotNullWhen(true)] out object? data,
                                  [NotNullWhen(true)] out string? fileTypeExtension)
    {
        if(!DataUrlInfo.TryParseInternal(ref dataUrl, out DataUrlInfo info))
        {
            data = fileTypeExtension = null;
            return false;
        }

        if(info.TryGetEmbeddedData(out data))
        {
            fileTypeExtension = info.GetFileTypeExtension();
            return true;
        }

        data = fileTypeExtension = null;
        return false;
    }

    /// <summary>
    /// Tries to parse a <see cref="string"/> that represents a "data" URL in order to make its content available as <see cref="DataUrlInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <param name="info">If the method returns <c>true</c>, the parameter contains a <see cref="DataUrlInfo"/> structure 
    /// that provides the contents of the "data" URL. The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrlInfo"/>, otherwise <c>false</c>.</returns>
    /// <seealso cref="TryParse(ReadOnlyMemory{char}, out DataUrlInfo)"/>
    public static bool TryParse(string? value, [NotNull] out DataUrlInfo info)
    {
        ReadOnlyMemory<char> mem = value.AsMemory();
        return DataUrlInfo.TryParseInternal(ref mem, out info);
    }


    /// <summary>
    /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that represents a "data" URL 
    /// in order to make its content available as <see cref="DataUrlInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
    /// <param name="info">If the method returns <c>true</c>, the parameter contains a <see cref="DataUrlInfo"/> 
    /// structure that provides the contents
    /// of the "data" URL. The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrlInfo"/>, <c>false</c> otherwise.</returns>
    /// <seealso cref="TryParse(string?, out DataUrlInfo)"/>
    public static bool TryParse(ReadOnlyMemory<char> value, [NotNull] out DataUrlInfo info) => DataUrlInfo.TryParseInternal(ref value, out info);
}
