using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;
using FolkerKinzel.Uris.Properties;

#if NET461 || NETSTANDARD2_0
using FolkerKinzel.Strings.Polyfills;
#endif

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
        private const int BASE64_LENGTH = 7;

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
        /// Returns an empty <see cref="DataUrl"/>.
        /// </summary>
        public static DataUrl Empty => default;

        #endregion

        #region Parser

        /// <summary>
        /// Tries to parse a <see cref="string"/> as <see cref="DataUrl"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <param name="dataUrl">If the method returns <c>true</c> the parameter contains a <see cref="DataUrl"/> struct that provides the contents
        /// of value. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrl"/>, <c>false</c> otherwise.</returns>
        public static bool TryParse(string? value, out DataUrl dataUrl)
        {
            dataUrl = default;
            DataEncoding dataEncoding = DataEncoding.UrlEncoded;

            if (!value.IsDataUrl())
            {
                return false;
            }


            int mimeTypeEndIndex = -1;
            int startOfData = -1;

            for (int i = PROTOCOL_LENGTH; i < value.Length; i++)
            {
                char c = value[i];
                if (char.IsWhiteSpace(c))
                {
                    return false;
                }

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
            ReadOnlySpan<char> mimePart = value.AsSpan(PROTOCOL_LENGTH, mimeTypeEndIndex - PROTOCOL_LENGTH);
            if (HasBase64Encoding(mimePart))
            {
                mimePart = mimePart.Slice(0, mimePart.Length - BASE64_LENGTH);
                mimeTypeEndIndex -= BASE64_LENGTH;
                dataEncoding = DataEncoding.Base64;
            }

            MimeType mediaType;

            if (mimePart.Trim().IsEmpty)
            {
                mediaType = DataUrl.DefaultMediaType();
            }
            else if (!MimeType.TryParse(value[PROTOCOL_LENGTH] == ';'
                ? new StringBuilder(10 + mimePart.Length)
                    .Append(stackalloc char[] { 't', 'e', 'x', 't', '/', 'p', 'l', 'a', 'i', 'n' })
                    .Append(mimePart).ToString()
                    .AsMemory()
                : value.AsMemory(PROTOCOL_LENGTH, mimeTypeEndIndex - PROTOCOL_LENGTH), out mediaType))
            {
                return false;
            }

            dataUrl = new DataUrl(mediaType, dataEncoding, value.AsMemory(startOfData + 1));

            return true;

            //////////////////////////////////////////////////////////////

            static bool HasBase64Encoding(ReadOnlySpan<char> val)
            {
                //Suche ";base64"
                if (val.Length < BASE64_LENGTH)
                {
                    return false;
                }

                ReadOnlySpan<char> hayStack = val.Slice(val.Length - BASE64_LENGTH);

                ReadOnlySpan<char> base64 = stackalloc char[] { ';', 'b', 'a', 's', 'e', '6', '4' };

                for (int i = 0; i < hayStack.Length; i++)
                {
                    char c = char.ToLowerInvariant(hayStack[i]);

                    if (c != base64[i])
                    {
                        return false;
                    }
                }

                return true;
            }

        }


        #endregion

        #region Builder

        /// <summary>
        /// Embeds Text in a "data" URL (RFC 2397).
        /// </summary>
        /// <param name="text">The text to embed into the "data" URL.</param>
        /// <returns>A "data" URL, into which the text provided by the parameter <paramref name="text"/> is embedded.</returns>
        /// <exception cref="FormatException">The <see cref="Uri"/> class was not able to encode <paramref name="text"/> correctly.</exception>
        public static string FromText(string? text)
        {
            string data = text is null ? string.Empty : Uri.EscapeDataString(Uri.UnescapeDataString(text));

            var sb = new StringBuilder(PROTOCOL_LENGTH + 1 + data.Length);

            return sb.AppendProtocol().Append(',').Append(data).ToString();

            // $"data:,{Uri.EscapeDataString(text)}"
        }


        /// <summary>
        /// Embeds binary data in a "data" URL (RFC 2397).
        /// </summary>
        /// <param name="bytes">The binary data to embed into the "data" URL.</param>
        /// <param name="mediaType">The <see cref="MimeType"/> of the data passed to the parameter <paramref name="bytes"/>.</param>
        /// <returns>A "data" URL, into which the binary data provided by the parameter <paramref name="bytes"/> is embedded.</returns>
        public static string FromBytes(byte[]? bytes, MimeType mediaType)
        {
            string data = bytes is null ? string.Empty : Convert.ToBase64String(bytes);

            var builder = new StringBuilder(PROTOCOL_LENGTH + MimeType.StringLength + BASE64_LENGTH + 1 + data.Length);

            return builder.AppendProtocol().AppendMediaType(mediaType).AppendBase64().Append(',').Append(data).ToString();

            // $"data:{mediaTypeString};base64,{Convert.ToBase64String(bytes)}"
        }




        ///// <summary>
        ///// Erzeugt einen <see cref="Uri"/>, in den der Inhalt einer Datei eingebettet ist.
        ///// </summary>
        ///// <param name="path">Absoluter Pfad zu der einzubettenden Datei.</param>
        ///// <param name="mediaType">Der <see cref="MimeType"/> der einzubettenden Datei oder <c>null</c>. Wenn <c>null</c> angegeben wird,
        ///// wird versucht, den <see cref="MimeType"/> aus der Dateiendung automatisch zu ermitteln.</param>
        ///// <returns>Ein <see cref="DataUrlBuilder"/>, in den die Daten der mit <paramref name="path"/> referenzierten Datei
        ///// eingebettet sind.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="path"/> ist <c>null</c>.</exception>
        ///// <exception cref="ArgumentException"><paramref name="path"/> ist kein gültiger Dateipfad.</exception>
        ///// <exception cref="UriFormatException">Es kann kein <see cref="Uri"/> initialisiert werden, z.B.
        ///// weil der URI-String länger als 65519 Zeichen ist.</exception>
        ///// <exception cref="IOException">E/A-Fehler.</exception>
        //public static async Task<string> FromFileAsync(string path, MimeType? mediaType = null)
        //{
        //    byte[] bytes = await LoadFileAsync(path).ConfigureAwait(false);

        //    if (mediaType is null)
        //    {
        //        mediaType = MimeType.FromFileTypeExtension(Path.GetExtension(path));
        //    }

        //    return FromBytes(bytes, mediaType.Value);
        //}


        /// <summary>
        /// Embeds the content of a file in a "data" URL (RFC 2397).
        /// </summary>
        /// <param name="path">Abolute path to the file which content is to embed into the "data" URL.</param>
        /// <param name="mimeType">The <see cref="MimeType"/> of the file to embed or <c>null</c> to let the method automatically
        /// retrieve the <see cref="MimeType"/> from the file type extension.</param>
        /// <returns>A "data" URL, into which the content of the file provided by the parameter <paramref name="path"/> is embedded.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> is not a valid file path.</exception>
        /// <exception cref="IOException">I/O error.</exception>
        public static string FromFile(string path, MimeType? mimeType = null)
        {
            byte[] bytes = LoadFile(path);

            if (mimeType is null)
            {
                mimeType = MimeType.FromFileTypeExtension(Path.GetExtension(path));
            }

            return FromBytes(bytes, mimeType.Value);
        }

        #endregion

        #region Data

        /// <summary>
        /// Tries to retrieve the text that is embedded in the <see cref="DataUrl"/>.
        /// </summary>
        /// <param name="embeddedText">If the method returns <c>true</c> the parameter contains the text, which was embedded in the <see cref="DataUrl"/>.
        /// The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the data embedded in the data url could be parsed as text, <c>false</c> otherwise.</returns>
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
        /// Tries to retrieve the binary data that is embedded in the <see cref="DataUrl"/>.
        /// </summary>
        /// <param name="embeddedText">If the method returns <c>true</c> the parameter contains the binary data, which was embedded in the <see cref="DataUrl"/>.
        /// The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the data embedded in the data url could be parsed as binary data, <c>false</c> otherwise.</returns>
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
        /// Returns a suitable file type extension for the data embedded in the <see cref="DataUrl"/>. The file type extension contains the 
        /// period (".").
        /// </summary>
        /// <returns>A suitable file type extension for the data embedded in the <see cref="DataUrl"/>.</returns>
        /// <remarks>The search for a file type extension can be an expensive operation. To make subsequent calls of the method faster, the
        /// recent file type extensions are stored in a cache. You can call <see cref="MimeType.ClearCache"/> to clear this cache.</remarks>
        public string GetFileTypeExtension() => MimeType.GetFileTypeExtension();

        #endregion

        [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal static MimeType DefaultMediaType()
        {
            _ = MimeType.TryParse(DEFAULT_MEDIA_TYPE.AsMemory(), out MimeType mediaType);
            return mediaType;
        }

        #region private

        //    private static async Task<byte[]> LoadFileAsync(string path)
        //    {
        //        try
        //        {
        //#if NETSTANDARD2_0 || NET461
        //                return await Task.Run(() => File.ReadAllBytes(path)).ConfigureAwait(false);
        //#else
        //            return await File.ReadAllBytesAsync(path).ConfigureAwait(false);
        //#endif
        //        }
        //        catch (ArgumentNullException)
        //        {
        //            throw new ArgumentNullException(nameof(path));
        //        }
        //        catch (ArgumentException e)
        //        {
        //            throw new ArgumentException(e.Message, nameof(path), e);
        //        }
        //        catch (UnauthorizedAccessException e)
        //        {
        //            throw new IOException(e.Message, e);
        //        }
        //        catch (NotSupportedException e)
        //        {
        //            throw new ArgumentException(e.Message, nameof(path), e);
        //        }
        //        catch (System.Security.SecurityException e)
        //        {
        //            throw new IOException(e.Message, e);
        //        }
        //        catch (PathTooLongException e)
        //        {
        //            throw new ArgumentException(e.Message, nameof(path), e);
        //        }
        //        catch (Exception e)
        //        {
        //            throw new IOException(e.Message, e);
        //        }
        //    }


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
