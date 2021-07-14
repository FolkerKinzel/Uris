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
        /// <summary>
        /// Gibt an, dass der <see cref="Uri"/> ein <see cref="DataUrlBuilder"/> nach RFC 2397 ist. Dieses Feld ist schreibgeschützt.
        /// </summary>
        private const string URI_SCHEME_DATA = "data:";

        public static bool IsDataUrl(this string? urlString) => urlString is not null && urlString.StartsWith(URI_SCHEME_DATA, StringComparison.OrdinalIgnoreCase);

        public static bool IsDataUrl(this Uri? dataUrl) => dataUrl is not null && dataUrl.OriginalString.IsDataUrl();


        /// <summary>
        /// Erstellt einen neuen <see cref="DataUrlBuilder"/>. Löst keine Ausnahme aus, wenn der <see cref="DataUrlBuilder"/> nicht erstellt werden kann.
        /// </summary>
        /// <param name="value">Ein <see cref="string"/>, der dem Data-URL-Schema nach RFC 2397 entspricht.</param>
        /// <returns>Ein <see cref="bool"/>-Wert, der <c>true</c> ist, wenn value erfolgreich als Data-Url geparst wurde, andernfalls <c>false</c>.</returns>
        public static bool TryParse(string? value, [NotNullWhen(true)] out DataUrlInfo? dataUrlInfo)
        {
            dataUrlInfo = null;
            DataEncoding dataEncoding = DataEncoding.UrlEncoded;
            string? embeddedData;

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

            embeddedData = value.Substring(startOfData + 1);

            dataUrlInfo = new DataUrlInfo(mediaType, dataEncoding, embeddedData);

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
