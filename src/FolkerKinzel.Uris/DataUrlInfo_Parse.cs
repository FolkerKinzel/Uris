using FolkerKinzel.Uris.Extensions;
using FolkerKinzel.Uris.Properties;

namespace FolkerKinzel.Uris;

public readonly partial struct DataUrlInfo
{
    /// <summary>
    /// Parses a <see cref="string"/> that represents a "data" URL as <see cref="DataUrlInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <returns>A <see cref="DataUrlInfo"/> instance, which represents <paramref name="value"/>.</returns>
    /// 
    /// <example>
    /// <note type="note">
    /// For the sake of better readability, exception handling is ommitted in the example.
    /// </note>
    /// <para>
    /// Creating and parsing a "data" URL:
    /// </para>
    /// <code language="c#" source="./../Examples/DataUrlExample.cs"/>
    /// </example>
    /// <seealso cref="Parse(ReadOnlyMemory{char})"/>
    /// <seealso cref="TryParse(string?, out DataUrlInfo)"/>
    /// <seealso cref="TryParse(ReadOnlyMemory{char}, out DataUrlInfo)"/>
    /// 
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> could not be parsed as <see cref="DataUrlInfo"/>.</exception>
    public static DataUrlInfo Parse(string value)
        => value is null
            ? throw new ArgumentNullException(nameof(value))
            : TryParse(value, out DataUrlInfo dataUrl)
                ? dataUrl
                : throw new ArgumentException(string.Format(Res.InvalidDataUrl, nameof(value)), nameof(value));


    /// <summary>
    /// Parses a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that represents a "data" URL as <see cref="DataUrlInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
    /// <returns>A <see cref="DataUrlInfo"/> instance, which represents <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="value"/> could not be parsed as <see cref="DataUrlInfo"/>.</exception>
    /// <seealso cref="Parse(string)"/>
    /// <seealso cref="TryParse(ReadOnlyMemory{char}, out DataUrlInfo)"/>
    /// <seealso cref="TryParse(string?, out DataUrlInfo)"/>
    public static DataUrlInfo Parse(ReadOnlyMemory<char> value)
        => TryParseInternal(ref value, out DataUrlInfo dataUrl)
                ? dataUrl
                : throw new ArgumentException(string.Format(Res.InvalidDataUrl, nameof(value)), nameof(value));


    /// <summary>
    /// Tries to parse a <see cref="string"/> that represents a "data" URL in order to make its content available as <see cref="DataUrlInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to parse.</param>
    /// <param name="info">If the method returns <c>true</c>, the parameter contains a <see cref="DataUrlInfo"/> structure 
    /// that provides the contents of the "data" URL. The argument is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrlInfo"/>, otherwise <c>false</c>.</returns>
    /// <seealso cref="TryParse(ReadOnlyMemory{char}, out DataUrlInfo)"/>
    public static bool TryParse(string? value, out DataUrlInfo info)
    {
        ReadOnlyMemory<char> mem = value.AsMemory();
        return TryParseInternal(ref mem, out info);
    }


    /// <summary>
    /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that represents a "data" URL 
    /// as <see cref="DataUrlInfo"/>.
    /// </summary>
    /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
    /// <param name="info">If the method returns <c>true</c>, the parameter contains a <see cref="DataUrlInfo"/> 
    /// structure that provides the contents
    /// of the "data" URL. The argument is passed uninitialized.</param>
    /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrlInfo"/>, <c>false</c> otherwise.</returns>
    /// <seealso cref="TryParse(string?, out DataUrlInfo)"/>
    public static bool TryParse(ReadOnlyMemory<char> value, out DataUrlInfo info) => TryParseInternal(ref value, out info);

    private static bool TryParseInternal(ref ReadOnlyMemory<char> value, out DataUrlInfo info)
    {
        value = value.Trim();
        ReadOnlySpan<char> span = value.Span;
        info = default;

        if (!span.IsDataUrl())
        {
            return false;
        }

        int mimeTypeEndIndex = -1;
        int startOfData = -1;

        for (int i = DataUrlBuilder.Protocol.Length; i < span.Length; i++)
        {
            char c = span[i];

            if (c == ',')
            {
                startOfData = mimeTypeEndIndex = i;
                break;
            }
        }

        if (mimeTypeEndIndex == -1) // missing ','
        {
            return false;
        }

        // dies ändert ggf. auch mimeTypeEndIndex
        ReadOnlySpan<char> mimePart = span.Slice(DataUrlBuilder.Protocol.Length, mimeTypeEndIndex - DataUrlBuilder.Protocol.Length);
        DataEncoding dataEncoding = DataEncoding.Url;

        if (mimePart.EndsWith(DataUrlBuilder.Base64, StringComparison.OrdinalIgnoreCase))
        {
            mimePart = mimePart.Slice(0, mimePart.Length - DataUrlBuilder.Base64.Length);
            mimeTypeEndIndex -= DataUrlBuilder.Base64.Length;
            dataEncoding = DataEncoding.Base64;
        }

        MimeTypeInfo mediaType;

        if (mimePart.IsEmpty)
        {
            mediaType = MimeTypeInfo.Parse(DataUrlBuilder.DEFAULT_MEDIA_TYPE);
        }
        else
        {
            // if text/plain is omitted and only the parameters are provided:
            ReadOnlyMemory<char> memory = span[DataUrlBuilder.Protocol.Length] == ';'
                                            ? new StringBuilder(DataUrlBuilder.DEFAULT_MEDIA_TYPE.Length + mimePart.Length)
                                                .Append(DataUrlBuilder.DEFAULT_MEDIA_TYPE)
                                                .Append(mimePart).ToString()
                                                .AsMemory()
                                            : value.Slice(DataUrlBuilder.Protocol.Length, mimeTypeEndIndex - DataUrlBuilder.Protocol.Length);

            if (!MimeTypeInfo.TryParse(memory, out mediaType))
            {
                return false;
            }
        }
        ReadOnlyMemory<char> embeddedData = value.Slice(startOfData + 1);
        info = new DataUrlInfo(in mediaType, dataEncoding, in embeddedData);

        return true;


        //////////////////////////////////////////////////////////////

        //static bool HasBase64Encoding(ReadOnlySpan<char> val)
        //{
        //    //Suche ";base64"
        //    if (val.Length < DataUrlBuilder.Base64.Length)
        //    {
        //        return false;
        //    }



        //    ReadOnlySpan<char> hayStack = val.Slice(val.Length - DataUrlBuilder.Base64.Length);

        //    for (int i = 0; i < hayStack.Length; i++)
        //    {
        //        char c = char.ToLowerInvariant(hayStack[i]);

        //        if (c != DataUrlBuilder.Base64[i])
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }

}
