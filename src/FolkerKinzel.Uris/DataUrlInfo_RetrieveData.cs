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
            byte[] data;
            try
            {
                data = Base64Parser.Parse(Data.ToString());
            }
            catch
            {
                return false;
            }

            int bomLength = GetEncoding(data, out Encoding enc);

            try
            {
                embeddedText = enc.GetString(data, bomLength, data.Length - bomLength);
            }
            catch
            {
                return false;
            }
        }
        else
        {
            // Url-Codierter UTF-8-String:
            embeddedText = Uri.UnescapeDataString(Data.ToString());
        }

        return true;
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

        if (!ContainsEmbeddedBytes)
        {
            return false;
        }

        try
        {
            if (this.DataEncoding == DataEncoding.Base64)
            {
                embeddedBytes = Base64Parser.Parse(Data.ToString());
            }
            else
            {
                byte[] bytes = DataUrlInfo.InitEncoding(ASCII_CODEPAGE).GetBytes(Data.ToString());
                embeddedBytes = WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length);
            }
        }
        catch
        {
            return false;
        }

        return true;
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
    public string GetFileTypeExtension() => MimeTypeInfo.GetFileTypeExtension();


    private int GetEncoding(byte[] data, out Encoding enc)
    {
        int codePage = TextEncodingConverter.GetCodePage(data, out int bomLength);

        MimeTypeParameterInfo charsetParameter = MimeTypeInfo.Parameters().FirstOrDefault(Predicate);

        enc = charsetParameter.IsEmpty
                        ? DataUrlInfo.InitEncoding(codePage)
                        : charsetParameter.IsAsciiCharSetParameter
                            ? DataUrlInfo.InitEncoding(ASCII_CODEPAGE)
                            : DataUrlInfo.InitEncoding(charsetParameter.Value.ToString());
        return bomLength;

        ////////////////////////////////////

        static bool Predicate(MimeTypeParameterInfo p) => p.IsCharSetParameter;
    }


    private static Encoding InitEncoding(int codePage) =>
        TextEncodingConverter.GetEncoding(codePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback, true);

    private static Encoding InitEncoding(string encodingName) =>
        TextEncodingConverter.GetEncoding(encodingName, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback, true);
}
