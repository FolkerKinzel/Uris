using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;
using FolkerKinzel.Uris.Properties;

namespace FolkerKinzel.Uris
{
    /// <summary>
    /// Kapselt die in einem Data-URL (RFC 2397) enthaltenen Informationen.
    /// </summary>
    public readonly struct DataUrl
    {
        private const string DEFAULT_MEDIA_TYPE = "text/plain";
        private const int PROTOCOL_LENGTH = 5;
        private const int BASE64_LENGTH = 8;
        
        private readonly ReadOnlyMemory<char> _embeddedData;

        internal DataUrl(InternetMediaType mediaType, DataEncoding dataEncoding, ReadOnlyMemory<char> embeddedData)
        {
            InternetMediaType = mediaType;
            DataEncoding = dataEncoding;
            _embeddedData = embeddedData;
        }

        /// <summary>
        /// Der Datentyp der im Data-URL eingebetteten Daten.
        /// </summary>
        public InternetMediaType InternetMediaType { get; }

        /// <summary>
        /// Die Art der Enkodierung der in <see cref="EmbeddedData"/> enthaltenen Daten.
        /// </summary>
        public DataEncoding DataEncoding { get; }


        /// <summary>
        /// Der Teil des Data-URLs, der die eingebetteten Daten enthält.
        /// </summary>
        public ReadOnlySpan<char> EmbeddedData => _embeddedData.Span;


        /// <summary>
        /// <c>true</c>, wenn der Data-Url eingebetteten Text enthält.
        /// </summary>
        public bool ContainsText => this.InternetMediaType.IsTextMediaType();


        /// <summary>
        /// <c>true</c>, wenn der Data-URL eingebettete binäre Daten enthält.
        /// </summary>
        public bool ContainsBytes => DataEncoding == DataEncoding.Base64 || !ContainsText;


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

        /// <summary>
        /// Erzeugt einen <see cref="Uri"/>, in den Text eingebettet ist.
        /// </summary>
        /// <param name="text">Der in den <see cref="Uri"/> einzubettende Text.</param>
        /// <returns>Ein <see cref="Uri"/>, in den <paramref name="text"/> eingebettet ist.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="text"/> ist ein Leerstring oder
        /// enthält nur Whitespace.</exception>
        public static string FromText(string text)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(Res.NoData, nameof(text));
            }

            string data = Uri.EscapeDataString(text);

            var sb = new StringBuilder(PROTOCOL_LENGTH + 1 + data.Length);

            return sb.AppendProtocol().Append(',').Append(data).ToString();

            // $"data:,{Uri.EscapeDataString(text)}"
        }


        /// <summary>
        /// Erzeugt einen <see cref="Uri"/>, in den binäre Daten eingebettet sind.
        /// </summary>
        /// <param name="bytes">Die in den <see cref="Uri"/> einzubettenden Daten.</param>
        /// <param name="mediaType">Der <see cref="InternetMediaType"/> der in <paramref name="bytes"/> enthaltenen
        /// Daten.</param>
        /// <returns>Ein <see cref="Uri"/>, in den die in <paramref name="bytes"/> enthaltenen 
        /// binären Daten eingebettet sind.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> oder <paramref name="mediaType"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="bytes"/> ist ein leeres Array.</exception>
        public static string FromBytes(byte[] bytes, InternetMediaType mediaType)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            //if (mediaType is null)
            //{
            //    throw new ArgumentNullException(nameof(mediaType));
            //}

            if (bytes.Length == 0)
            {
                throw new ArgumentException(Res.NoData, nameof(bytes));
            }

            string data = Convert.ToBase64String(bytes);

            var builder = new StringBuilder(PROTOCOL_LENGTH + BASE64_LENGTH + InternetMediaType.StringLength + data.Length);

            return builder.AppendProtocol().AppendMediaType(mediaType).AppendBase64().Append(data).ToString();

            // $"data:{mediaTypeString};base64,{Convert.ToBase64String(bytes)}"
        }

        


        /// <summary>
        /// Erzeugt einen <see cref="Uri"/>, in den der Inhalt einer Datei eingebettet ist.
        /// </summary>
        /// <param name="path">Absoluter Pfad zu der einzubettenden Datei.</param>
        /// <param name="mediaType">Der <see cref="InternetMediaType"/> der einzubettenden Datei oder <c>null</c>. Wenn <c>null</c> angegeben wird,
        /// wird versucht, den <see cref="InternetMediaType"/> aus der Dateiendung automatisch zu ermitteln.</param>
        /// <returns>Ein <see cref="DataUrlBuilder"/>, in den die Daten der mit <paramref name="path"/> referenzierten Datei
        /// eingebettet sind.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> ist kein gültiger Dateipfad.</exception>
        /// <exception cref="UriFormatException">Es kann kein <see cref="Uri"/> initialisiert werden, z.B.
        /// weil der URI-String länger als 65519 Zeichen ist.</exception>
        /// <exception cref="IOException">E/A-Fehler.</exception>
        public static async Task<string> FromFileAsync(string path, InternetMediaType? mediaType = null)
        {
            byte[] bytes = await LoadFileAsync(path).ConfigureAwait(false);

            if (mediaType is null)
            {
                mediaType = InternetMediaType.FromFileTypeExtension(Path.GetExtension(path));
            }

            return FromBytes(bytes, mediaType.Value);
        }


        public static string FromFile(string path, InternetMediaType? mediaType = null)
        {
            byte[] bytes = LoadFile(path);

            if (mediaType is null)
            {
                mediaType = InternetMediaType.FromFileTypeExtension(Path.GetExtension(path));
            }

            return FromBytes(bytes, mediaType.Value);
        }

        
        /// <summary>
        /// Ruft im Data-URL eingebetten Text ab.
        /// </summary>
        /// <param name="embeddedText">Enthält nach dem Beenden dieser Methode den im Data-URL eingebetteten Text, wenn 
        /// die eingebetteten Daten als Text geparst werden konnten, anderenfalls <c>null</c>. Dieser Parameter wird 
        /// uninitialisiert übergeben.</param>
        /// <returns><c>true</c>, wenn die im Data-Url eingebetteten Daten als Text geparst werden konnten, anderenfalls <c>false</c>.</returns>
        public bool TryGetEmbeddedText([NotNullWhen(true)] out string? embeddedText)
        {
            embeddedText = null;
            if (!ContainsText)
            {
                return false;
            }

            // als Base64 codierter Text:
            if (DataEncoding == DataEncoding.Base64)
            {
                static bool Predicate(MediaTypeParameter p) => p.IsCharsetParameter();

                MediaTypeParameter charsetParameter = InternetMediaType.Parameters.FirstOrDefault(Predicate);
                
                Encoding enc = charsetParameter.IsEmpty ? Encoding.ASCII : TextEncodingConverter.GetEncoding(charsetParameter.Value.ToString());
                
                try
                {
                    embeddedText = enc.GetString(Convert.FromBase64String(EmbeddedData.ToString()));
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                // Url-Codierter UTF-8-String:
                embeddedText = Uri.UnescapeDataString(EmbeddedData.ToString());
            }

            return true;
        }


        /// <summary>
        /// Gibt die im Data-Url eingebetteten binären Daten zurück oder <c>null</c>,
        /// wenn der Data-Url keine eingebetteten binären Daten enthält oder wenn
        /// diese nicht dekodiert werden konnten.
        /// </summary>
        /// <param name="embeddedBytes">
        /// Enthält nach dem Beenden dieser Methode die im Data-URL eingebetteten binären Daten, wenn 
        /// die eingebetteten Daten als binäre Daten geparst werden konnten, anderenfalls <c>null</c>. 
        /// Dieser Parameter wird uninitialisiert übergeben.</param>
        /// <returns><c>true</c>, wenn die im Data-Url eingebetteten Daten als binäre 
        /// Daten geparst werden konnten, anderenfalls <c>false</c>.</returns>
        public bool TryGetEmbeddedBytes([NotNullWhen(true)] out byte[]? embeddedBytes)
        {
            embeddedBytes = null;

            if (!ContainsBytes)
            {
                return false;
            }

            try
            {
                embeddedBytes = this.DataEncoding == DataEncoding.Base64
                    ? Convert.FromBase64String(EmbeddedData.ToString())
                    : Encoding.UTF8.GetBytes(Uri.UnescapeDataString(EmbeddedData.ToString()));
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Gibt eine geeignete Dateiendung für die in den den Data-Url eingebetteten Daten 
        /// zurück. Die Dateiendung enthält den Punkt "." als Trennzeichen.
        /// </summary>
        /// <returns>Ein <see cref="Task{TResult}"/>-Objekt, das den Zugriff auf eine geeignete Dateiendung für die in den Data-URL
        /// eingebetteten Daten ermöglicht.</returns>
        /// <remarks>Da das Auffinden einer geeigneten Dateiendung ein aufwändiger Vorgang ist, werden Suchergebnisse für eine
        /// kurze Zeitspanne in einem Cache zwischengespeichert, um die Performance zu erhöhen.</remarks>
        public string GetFileTypeExtension() => InternetMediaType.GetFileTypeExtension();

        
        [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal static InternetMediaType DefaultMediaType()
        {
            _ = InternetMediaType.TryParse(DEFAULT_MEDIA_TYPE.AsMemory(), out InternetMediaType mediaType);
            return mediaType;
        }

        #region private

        private static async Task<byte[]> LoadFileAsync(string path)
        {
            try
            {
#if NETSTANDARD2_0
                return await Task.Run(() => File.ReadAllBytes(path)).ConfigureAwait(false);
#else
                return await File.ReadAllBytesAsync(path).ConfigureAwait(false);
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
        }


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
}
