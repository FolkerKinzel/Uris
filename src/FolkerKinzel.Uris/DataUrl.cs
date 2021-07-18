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
    /// Represents a "data" URL (RFC 2397) that embeds data in-line in a URL.
    /// </summary>
    public readonly struct DataUrl
    {
        #region private const

        private const string DEFAULT_MEDIA_TYPE = "text/plain";
        private const int PROTOCOL_LENGTH = 5;
        private const int BASE64_LENGTH = 8;

        #endregion

        private readonly ReadOnlyMemory<char> _embeddedData;

        internal DataUrl(MimeType mediaType, DataEncoding dataEncoding, ReadOnlyMemory<char> embeddedData)
        {
            MimeType = mediaType;
            DataEncoding = dataEncoding;
            _embeddedData = embeddedData;
        }

        #region Properties

        /// <summary>
        /// The data type of the embedded data.
        /// </summary>
        public MimeType MimeType { get; }

        /// <summary>
        /// The encoding of the data in <see cref="EmbeddedData"/>.
        /// </summary>
        public DataEncoding DataEncoding { get; }


        /// <summary>
        /// The part of the "data" URL, which contains the embedded data.
        /// </summary>
        public ReadOnlySpan<char> EmbeddedData => _embeddedData.Span;


        /// <summary>
        /// <c>true</c> if <see cref="EmbeddedData"/> contains text.
        /// </summary>
        public bool ContainsText => this.MimeType.IsTextMediaType();


        /// <summary>
        /// <c>true</c> if <see cref="EmbeddedData"/> contains binary data.
        /// </summary>
        public bool ContainsBytes => DataEncoding == DataEncoding.Base64 || !ContainsText;

        /// <summary>
        /// <c>true</c> if the <see cref="DataUrl"/> contains nothing.
        /// </summary>
        public bool IsEmpty => this.MimeType.IsEmpty;

        /// <summary>
        /// Returns an empty <see cref="DataUrl"/> struct.
        /// </summary>
        public static DataUrl Empty => default;

        #endregion

        #region Parser

        /// <summary>
        /// Tries to parse a <see cref="string"/> as <see cref="DataUrl"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse., der dem Data-URL-Schema nach RFC 2397 entspricht.</param>
        /// <param name="dataUrl">If the method returns <c>true</c> the parameter contains a <see cref="DataUrl"/> struct that provides the contents
        /// of value. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if  <paramref name="value"/> could be parsed as <see cref="DataUrl"/>, <c>false</c> otherwise.</returns>
        public static bool TryParse(string? value, out DataUrl dataUrl)
        {
            dataUrl = default;
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

            MimeType mediaType;

            if (value.AsSpan(DATA_PROTOCOL_LENGTH, endIndex - DATA_PROTOCOL_LENGTH).Trim().IsEmpty)
            {
                mediaType = DataUrl.DefaultMediaType();
            }
            else if (!MimeType.TryParse(value[DATA_PROTOCOL_LENGTH] == ';'
                ? ("text/plain" + value.Substring(DATA_PROTOCOL_LENGTH, endIndex - DATA_PROTOCOL_LENGTH)).AsMemory()
                : value.AsMemory(DATA_PROTOCOL_LENGTH, endIndex - DATA_PROTOCOL_LENGTH), out mediaType))
            {
                return false;
            }

            dataUrl = new DataUrl(mediaType, dataEncoding, value.AsMemory(startOfData + 1));

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

        #endregion

        #region Builder

        /// <summary>
        /// Creates a "data" URL (RFC 2397), which contains embedded text.
        /// </summary>
        /// <param name="text">The text to embed into the "data" URL.</param>
        /// <returns>A "data" URL, into which the text provided by the parameter text is embedded.</returns>
        /// <exception cref="FormatException">The <see cref="Uri"/> class was not able to encode <paramref name="text"/> correctly.</exception>
        ///// <exception cref="ArgumentNullException"><paramref name="text"/> ist <c>null</c>.</exception>
        ///// <exception cref="ArgumentException"><paramref name="text"/> ist ein Leerstring oder
        ///// enthält nur Whitespace.</exception>
        public static string FromText(string? text)
        {
            //if (text is null)
            //{
            //    throw new ArgumentNullException(nameof(text));
            //}

            //if (string.IsNullOrWhiteSpace(text))
            //{
            //    throw new ArgumentException(Res.NoData, nameof(text));
            //}

            //string data = text is null ? string.Empty : Uri.EscapeDataString(text);

            string data = text is null ? string.Empty : Uri.EscapeDataString(Uri.UnescapeDataString(text));

            var sb = new StringBuilder(PROTOCOL_LENGTH + 1 + data.Length);

            return sb.AppendProtocol().Append(',').Append(data).ToString();

            // $"data:,{Uri.EscapeDataString(text)}"
        }


        /// <summary>
        /// Erzeugt einen <see cref="Uri"/>, in den binäre Daten eingebettet sind.
        /// </summary>
        /// <param name="bytes">Die in den <see cref="Uri"/> einzubettenden Daten.</param>
        /// <param name="mediaType">Der <see cref="MimeType"/> der in <paramref name="bytes"/> enthaltenen
        /// Daten.</param>
        /// <returns>Ein <see cref="Uri"/>, in den die in <paramref name="bytes"/> enthaltenen 
        /// binären Daten eingebettet sind.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> oder <paramref name="mediaType"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="bytes"/> ist ein leeres Array.</exception>
        public static string FromBytes(byte[] bytes, MimeType mediaType)
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

            var builder = new StringBuilder(PROTOCOL_LENGTH + BASE64_LENGTH + MimeType.StringLength + data.Length);

            return builder.AppendProtocol().AppendMediaType(mediaType).AppendBase64().Append(data).ToString();

            // $"data:{mediaTypeString};base64,{Convert.ToBase64String(bytes)}"
        }

        


        /// <summary>
        /// Erzeugt einen <see cref="Uri"/>, in den der Inhalt einer Datei eingebettet ist.
        /// </summary>
        /// <param name="path">Absoluter Pfad zu der einzubettenden Datei.</param>
        /// <param name="mediaType">Der <see cref="MimeType"/> der einzubettenden Datei oder <c>null</c>. Wenn <c>null</c> angegeben wird,
        /// wird versucht, den <see cref="MimeType"/> aus der Dateiendung automatisch zu ermitteln.</param>
        /// <returns>Ein <see cref="DataUrlBuilder"/>, in den die Daten der mit <paramref name="path"/> referenzierten Datei
        /// eingebettet sind.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> ist kein gültiger Dateipfad.</exception>
        /// <exception cref="UriFormatException">Es kann kein <see cref="Uri"/> initialisiert werden, z.B.
        /// weil der URI-String länger als 65519 Zeichen ist.</exception>
        /// <exception cref="IOException">E/A-Fehler.</exception>
        public static async Task<string> FromFileAsync(string path, MimeType? mediaType = null)
        {
            byte[] bytes = await LoadFileAsync(path).ConfigureAwait(false);

            if (mediaType is null)
            {
                mediaType = MimeType.FromFileTypeExtension(Path.GetExtension(path));
            }

            return FromBytes(bytes, mediaType.Value);
        }


        public static string FromFile(string path, MimeType? mediaType = null)
        {
            byte[] bytes = LoadFile(path);

            if (mediaType is null)
            {
                mediaType = MimeType.FromFileTypeExtension(Path.GetExtension(path));
            }

            return FromBytes(bytes, mediaType.Value);
        }

        #endregion

        #region Data

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
                static bool Predicate(MimeTypeParameter p) => p.IsCharsetParameter();

                MimeTypeParameter charsetParameter = MimeType.Parameters.FirstOrDefault(Predicate);
                
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
        public string GetFileTypeExtension() => MimeType.GetFileTypeExtension();

        #endregion

        [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal static MimeType DefaultMediaType()
        {
            _ = MimeType.TryParse(DEFAULT_MEDIA_TYPE.AsMemory(), out MimeType mediaType);
            return mediaType;
        }

        #region private

        private static async Task<byte[]> LoadFileAsync(string path)
        {
            try
            {
#if NETSTANDARD2_0 || NET461
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
