using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.URIs.Properties;

namespace FolkerKinzel.URIs
{
    /// <summary>
    /// Repräsentiert einen Data-URL nach RFC 2397.
    /// </summary>
    public static class DataUrlHelper
    {
        private static readonly InternetMediaType _defaultMediaType = new();

        /// <summary>
        /// Gibt an, dass der <see cref="Uri"/> ein <see cref="DataUrlHelper"/> nach RFC 2397 ist. Dieses Feld ist schreibgeschützt.
        /// </summary>
        private const string UriSchemeData = "data:";

        ///// <summary>
        ///// Gibt das Zeichen an, das das Schema des Kommunikationsprotokolls vom Adressteil des URIs trennt. Dieses Feld ist schreibgeschützt.
        ///// </summary>
        //private const string SchemeDelimiter = ":";


        public static bool IsDataUrl(this string? urlString) => urlString is not null && urlString.StartsWith(UriSchemeData, StringComparison.OrdinalIgnoreCase);

        public static bool IsDataUrl(this Uri? dataUrl) => dataUrl is not null && dataUrl.OriginalString.IsDataUrl();


        /// <summary>
        /// Erstellt einen neuen <see cref="DataUrlHelper"/>. Löst keine Ausnahme aus, wenn der <see cref="DataUrlHelper"/> nicht erstellt werden kann.
        /// </summary>
        /// <param name="value">Ein <see cref="string"/>, der dem Data-URL-Schema nach RFC 2397 entspricht.</param>
        /// <returns>Ein <see cref="bool"/>-Wert, der <c>true</c> ist, wenn value erfolgreich als Data-Url geparst wurde, andernfalls <c>false</c>.</returns>
        public static bool TryParse(string? value, [NotNullWhen(true)] out DataUrlInfo? dataUrlInfo)
        {
            dataUrlInfo = null;
            DataEncoding dataEncoding = DataEncoding.UrlEncoded;
            InternetMediaType? mediaType;
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

            string mimeString = value.Substring(DATA_PROTOCOL_LENGTH, endIndex - DATA_PROTOCOL_LENGTH);

            if (string.IsNullOrWhiteSpace(mimeString))
            {
                mediaType = _defaultMediaType;
            }
#if NETSTANDARD2_0
            else if (!InternetMediaType.TryParse(mimeString.StartsWith(";") ? "text/plain" + mimeString : mimeString, out mediaType))
#else
            else if(!InternetMediaType.TryParse(mimeString.StartsWith(';') ? "text/plain" + mimeString : mimeString, out mediaType))
#endif
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


        /// <summary>
        /// Erzeugt einen <see cref="Uri"/>, in den beliebiger Text eingebettet ist.
        /// </summary>
        /// <param name="text">Der in den <see cref="Uri"/> einzubettende Text.</param>
        /// <returns>Ein <see cref="Uri"/>, in den <paramref name="text"/> eingebettet ist.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> ist ein Leerstring oder
        /// enthält nur Whitespace.</exception>
        /// <exception cref="UriFormatException">Es kann kein <see cref="Uri"/> initialisiert werden, z.B.
        /// weil der URI-String länger als 65519 Zeichen ist.</exception>
        public static Uri FromText(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(Res.NoData, nameof(text));
            }

            return new Uri($"data:,{Uri.EscapeDataString(text)}");
        }


        /// <summary>
        /// Erzeugt einen <see cref="Uri"/>, in den binäre Daten eingebettet sind.
        /// </summary>
        /// <param name="bytes">Die in den <see cref="Uri"/> einzubettenden Daten.</param>
        /// <param name="mimeType">Der MIME-Typ der in <paramref name="bytes"/> enthaltenen
        /// Daten oder <c>null</c> für "text/plain;charset=us-ascii".</param>
        /// <returns>Ein <see cref="DataUrlHelper"/>, in den die in <paramref name="bytes"/> enthaltenen 
        /// binären Daten eingebettet sind.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> oder <paramref name="mimeType"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="bytes"/> ist ein leeres Array oder <paramref name="mimeType"/> ist kein gültiger
        /// Internet-Medientyp.</exception>
        /// <exception cref="UriFormatException">Es kann kein <see cref="Uri"/> initialisiert werden, z.B.
        /// weil der URI-String länger als 65519 Zeichen ist.</exception>
        public static Uri FromBytes(byte[] bytes, InternetMediaType mediaType)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (mediaType is null)
            {
                throw new ArgumentNullException(nameof(mediaType));
            }

            if (bytes.Length == 0)
            {
                throw new ArgumentException(Res.NoData, nameof(bytes));
            }

            string mediaTypeString = mediaType == _defaultMediaType ? string.Empty :
                mediaType.MediaType == InternetMediaType.TEXT_MEDIA_TYPE && mediaType.SubType == InternetMediaType.PLAIN_SUB_TYPE
#if NETSTANDARD2_0
                ? $";{mediaType.ToString().Split(new char[] { ';' }, 2, StringSplitOptions.None)[1]}"
#else
                ? $";{mediaType.ToString().Split(';', 2, StringSplitOptions.None)[1]}"
#endif
                : mediaType.ToString();

            return new Uri($"data:{mediaTypeString};base64,{Convert.ToBase64String(bytes)}");
        }


        /// <summary>
        /// Erstellt einen <see cref="DataUrlHelper"/> aus einer physisch vorhandenen Datei.
        /// </summary>
        /// <param name="path">Absoluter Pfad zu der einzubettenden Datei.</param>
        /// <param name="mimeType">MIME-Typ der einzubettenden Datei. Wenn <c>null</c> angegeben wird,
        /// wird versucht, den MIME-Typ aus der Dateiendung automatisch zu ermitteln.</param>
        /// <returns>Ein <see cref="DataUrlHelper"/>, in den die Daten der mit <paramref name="path"/> referenzierten Datei
        /// eingebettet sind.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> oder <paramref name="mimeType"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> ist kein gültiger Dateipfad oder
        /// <paramref name="mimeType"/> hat kein gültiges Format.</exception>
        /// <exception cref="UriFormatException">Es kann kein <see cref="DataUrlHelper"/> initialisiert werden, z.B.
        /// weil der URI-String länger als 65519 Zeichen ist.</exception>
        /// <exception cref="IOException">E/A-Fehler.</exception>
        public static async Task<Uri> FromFileAsync(string path, InternetMediaType? mediaType = null)
        {
            byte[] bytes;
            try
            {
#if NETSTANDARD2_0
                bytes = await Task.Run(() => File.ReadAllBytes(path));
#else
                bytes = await File.ReadAllBytesAsync(path);
#endif
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

            if (mediaType is null)
            {
                mediaType = await InternetMediaType.GetFromFileTypeExtensionAsync(Path.GetExtension(path));
            }

            return DataUrlHelper.FromBytes(bytes, mediaType);
        }




    }
}
