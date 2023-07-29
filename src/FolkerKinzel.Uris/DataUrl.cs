using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris;

/// <summary>
/// Provides methods to handle <see cref="string"/>s that represent "data" URLs (RFC 2397).
/// </summary>
public static class DataUrl
{
    /// <summary>
    /// The data protocol that indicates a "data" Url (RFC 2397).
    /// </summary>
    public const string Protocol = "data:";

    /// <summary>
    /// The default Internet Media Type for a "data" Url (RFC 2397) if no other is specified.
    /// </summary>
    public const string DefaultMediaType = "text/plain";

    internal const string Base64 = ";base64";

    /// <summary>
    /// Embeds text into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="text">The text to embed into the "data" URL. <paramref name="text"/> MUST not be URL encoded.</param>
    /// <param name="mimeTypeString">The Internet Media Type of the <paramref name="text"/> or <c>null</c> for <see cref="DataUrl.DefaultMediaType"/>.</param>
    /// 
    /// <returns>A "data" URL, into which <paramref name="text"/> is embedded.</returns>
    /// <exception cref="FormatException"><paramref name="text"/> consists only of ASCII characters and the <see cref="Uri"/> class 
    /// was not able to encode <paramref name="text"/> correctly.</exception>
    /// <remarks>If <paramref name="text"/>
    /// consists only of ASCII characters the method serializes it Url encoded, otherwise <paramref name="text"/> is serialized
    /// as Base64 using <see cref="UTF8Encoding"/>.</remarks>
    [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
    public static string FromText(string? text, string? mimeTypeString = DataUrl.DefaultMediaType) =>
        MimeType.TryParse(string.IsNullOrWhiteSpace(mimeTypeString) ? DataUrl.DefaultMediaType : mimeTypeString, out MimeType? mimeType)
                ? FromText(text, mimeType)
                : FromText(text, DataUrl.DefaultMediaType);


    /// <summary>
    /// Embeds text into a "data" URL (RFC 2397).
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FromText(string? text, MimeType mimeType) => DataUrlBuilder.FromText(text, mimeType);
    


    /// <summary>
    /// Embeds binary data into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="bytes">The binary data to embed into the "data" URL.</param>
    /// <param name="mimeTypeString">The Internet Media Type of the <paramref name="bytes"/> or <c>null</c> for <see cref="MimeString.OctetStream"/>.</param>
    /// <returns>A "data" URL, into which the binary data provided by the parameter <paramref name="bytes"/> is embedded.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="mimeTypeString"/> is <c>null</c>.</exception>
    [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
    public static string FromBytes(byte[]? bytes, string? mimeTypeString = MimeString.OctetStream) =>
        MimeType.TryParse(string.IsNullOrWhiteSpace(mimeTypeString) ? MimeString.OctetStream : mimeTypeString, out MimeType? mimeType)
                ? FromBytes(bytes, mimeType)
                : FromBytes(bytes, MimeString.OctetStream);


    /// <summary>
    /// Embeds binary data into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="bytes">The binary data to embed into the "data" URL.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the <paramref name="bytes"/>.</param>
    /// <returns>A "data" URL, into which the binary data provided by the parameter <paramref name="bytes"/> is embedded.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="mimeType"/> is <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FromBytes(byte[]? bytes, MimeType mimeType) => DataUrlBuilder.FromBytes(bytes, mimeType);
    


    /// <summary>
    /// Embeds the content of a file into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="filePath">Path to the file whose content has to be embedded into the "data" URL.</param>
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
        string.IsNullOrWhiteSpace(mimeTypeString)
              ? FromFile(filePath, MimeType.FromFileName(filePath))
              : MimeType.TryParse(mimeTypeString, out MimeType? mimeType)
                  ? FromFile(filePath, mimeType)
                  : FromFile(filePath, (string?)null);


    /// <summary>
    /// Embeds the content of a file into a "data" URL (RFC 2397).
    /// </summary>
    /// <param name="filePath">Path to the file whose content has to be embedded into the "data" URL.</param>
    /// <param name="mimeType">The <see cref="MimeType"/> of the file to embed.</param>
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FromFile(string filePath, MimeType mimeType) => DataUrlBuilder.FromFile(filePath, mimeType);


    /// <summary>
    /// Tries to retrieve the embedded data from the <paramref name="dataUrl"/>.
    /// </summary>
    /// <param name="dataUrl">A "data" URL according to RFC 2397.</param>
    /// <param name="data">The embedded data. This can be either a <see cref="string"/> or a byte 
    /// array. The parameter is passed uninitialized.</param>
    /// <param name="fileTypeExtension">The file type extension for <paramref name="data"/>.
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
    /// <param name="fileTypeExtension">The file type extension for <paramref name="data"/>. The parameter 
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
    public static bool TryParse(string? value, out DataUrlInfo info)
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
    public static bool TryParse(ReadOnlyMemory<char> value, out DataUrlInfo info) => DataUrlInfo.TryParseInternal(ref value, out info);
}
