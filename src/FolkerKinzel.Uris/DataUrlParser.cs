using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;

#if NETSTANDARD2_0
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    public static class DataUrlParser
    {
        public static bool IsDataUrl(this string? urlString) => urlString.StartsWithDataUrlProtocol();

        public static bool IsDataUrl(this Uri? dataUrl) => dataUrl is not null && dataUrl.OriginalString.IsDataUrl();


        /// <summary>
        /// Erstellt einen neuen <see cref="DataUrlBuilder"/>. Löst keine Ausnahme aus, wenn der <see cref="DataUrlBuilder"/> nicht erstellt werden kann.
        /// </summary>
        /// <param name="value">Ein <see cref="string"/>, der dem Data-URL-Schema nach RFC 2397 entspricht.</param>
        /// <returns>Ein <see cref="bool"/>-Wert, der <c>true</c> ist, wenn value erfolgreich als Data-Url geparst wurde, andernfalls <c>false</c>.</returns>
        public static bool TryParse(string? value, [NotNullWhen(true)] out DataUrl? dataUrlInfo)
        {
            dataUrlInfo = null;
            DataEncoding dataEncoding = DataEncoding.UrlEncoded;

            if (value is null || !value.IsDataUrl())
            {
                return false;
            }

            const int DATA_PROTOCOL_LENGTH = 5; // 5 == LengthOf "data:"
            const int BASE64_LENGTH = 7; // 7 = LengthOf ";base64"

            int endIndex = -1;
            int startOfData = -1;

            for (int i = DATA_PROTOCOL_LENGTH; i < value.Length; i++)
            {
                char c = value[i];
                if (char.IsWhiteSpace(c))
                {
                    return false;
                }

                if (c == ',')
                {
                    startOfData = endIndex = i;
                    break;
                }
            }

            if (endIndex == -1)
            {
                return false;
            }

            // dies ändert ggf. auch endIndex
            if (HasBase64Encoding(value))
            {
                dataEncoding = DataEncoding.Base64;
            }

            InternetMediaType mediaType;

            if (value.AsSpan(DATA_PROTOCOL_LENGTH, endIndex - DATA_PROTOCOL_LENGTH).Trim().IsEmpty)
            {
                mediaType = DataUrl.DefaultMediaType();
            }
            else if (!InternetMediaType.TryParse(value[DATA_PROTOCOL_LENGTH] == ';'
                ? ("text/plain" + value.Substring(DATA_PROTOCOL_LENGTH, endIndex - DATA_PROTOCOL_LENGTH)).AsMemory()
                : value.AsMemory(DATA_PROTOCOL_LENGTH, endIndex - DATA_PROTOCOL_LENGTH), out mediaType))
            {
                return false;
            }

            dataUrlInfo = new DataUrl(mediaType, dataEncoding, value.AsMemory(startOfData + 1));

            return true;

            //////////////////////////////////////////////////////////////

            bool HasBase64Encoding(string val)
            {
                //Suche ";base64"
                int start = endIndex - 1;
                int end = endIndex - BASE64_LENGTH;

                if (end > DATA_PROTOCOL_LENGTH)
                {
                    int index = BASE64_LENGTH - 1;

                    for (int i = start; i >= end; i--, index--)
                    {
                        char c = char.ToLowerInvariant(val[i]);

                        if (c != ";base64"[index])
                        {
                            return false;
                        }
                    }

                    endIndex = end;
                    return true;
                }

                return false;
            }
        }
    }
}
