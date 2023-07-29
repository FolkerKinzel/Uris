using System.Net;
using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris;

public readonly partial struct DataUrlInfo
{
    private const int ASCII_CODEPAGE = 20127;

    /// <summary>
    /// Tries to retrieve the text, which is embedded in the "data" URL.
    /// </summary>
    /// <param name="embeddedText">If the method returns <c>true</c> the parameter contains the text, which was embedded in the <see cref="DataUrlInfo"/>.
    /// The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if the data embedded in the data url could be parsed as text, <c>false</c> otherwise.</returns>
    public bool TryGetEmbeddedText([NotNullWhen(true)] out string? embeddedText)
    {
        embeddedText = null;

        if (!ContainsEmbeddedText)
        {
            return false;
        }

        // als Base64 codierter Text:
        if (DataEncoding == DataEncoding.Base64)
        {
            if (!Base64Parser.TryDecode(Data, out byte[]? data))
            {
                return false;
            }

            int bomLength = GetEncoding(data, out Encoding enc);

            try
            {
                embeddedText = enc.GetString(data, bomLength, data.Length - bomLength);
                return true;
            }
            catch
            {
                return false;
            }
        }
        else
        {
            // URL encoded String:
            string? encodingName = TryGetEncodingFromMimeType(out encodingName) ? encodingName : DataUrlBuilder.UTF_8;
            return UrlEncoding.TryDecode(Data.ToString(), encodingName, out embeddedText);
        }
    }

    /// <summary>
    /// Tries to retrieve the binary data, which is embedded in the "data" URL.
    /// </summary>
    /// <param name="embeddedBytes">If the method returns <c>true</c> the parameter contains the binary data, which was embedded in the <see cref="DataUrlInfo"/>.
    /// The parameter is passed uninitialized.</param>
    /// <returns><c>true</c> if the data embedded in the data url could be parsed as binary data, <c>false</c> otherwise.</returns>
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
    public bool TryGetEmbeddedBytes([NotNullWhen(true)] out byte[]? embeddedBytes)
    {
        embeddedBytes = null;

        return ContainsEmbeddedBytes &&
               (this.DataEncoding == DataEncoding.Base64
                    ? Base64Parser.TryDecode(Data, out embeddedBytes)
                    : UrlEncoding.TryDecodeBytes(Data, out embeddedBytes));
    }


    /// <summary>
    /// Returns an appropriate file type extension for the data embedded in the "data" URL. The file type extension contains the 
    /// period (".").
    /// </summary>
    /// <returns>An appropriate file type extension for the data embedded in the <see cref="DataUrlInfo"/>.</returns>
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetFileTypeExtension() => MimeString.ToFileTypeExtension(MimeType.Span);

    private int GetEncoding(byte[] data, out Encoding enc)
    {
        int codePage = TextEncodingConverter.GetCodePage(data, out int bomLength);

        enc = TryGetEncodingFromMimeType(out string? charsetName)
                   ? TextEncoding.InitThrowingEncoding(charsetName)
                   : TextEncoding.InitThrowingEncoding(codePage);

        return bomLength;
    }

    private bool TryGetEncodingFromMimeType([NotNullWhen(true)] out string? encodingName)
    {
        if (!MimeTypeInfo.TryParse(MimeType, out MimeTypeInfo info))
        {
            encodingName = null;
            return false;
        }

        MimeTypeParameterInfo charsetPara = info.Parameters().FirstOrDefault(Predicate);
        if(charsetPara.IsEmpty)
        {
            encodingName = null;
            return false;
        }
        encodingName = charsetPara.Value.ToString();
        return true;

        ///////////////////////////////////////////////////////////

        static bool Predicate(MimeTypeParameterInfo p) => p.IsCharSetParameter;
    }



    
}
