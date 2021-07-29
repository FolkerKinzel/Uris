using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public readonly struct DataUrl : IEquatable<DataUrl>
    {
        #region private const

        private const string BASE64 = ";base64";
        internal const string PROTOCOL = "data:";
        private const string DEFAULT_MEDIA_TYPE = "text/plain";

        #endregion

        private readonly ReadOnlyMemory<char> _embeddedData;

        internal DataUrl(in MimeType mediaType, DataEncoding dataEncoding, in ReadOnlyMemory<char> embeddedData)
        {
            MimeType = mediaType;
            DataEncoding = dataEncoding;
            _embeddedData = embeddedData;
        }

        #region Properties

        /// <summary>
        /// The internet media type of the embedded data.
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
        public bool ContainsText => this.MimeType.IsText();


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

        #region IEquatable


        #region Operators

        /// <summary>
        /// Returns a value that indicates whether the values of two specified <see cref="DataUrl"/> instances are equal.
        /// </summary>
        /// <param name="dataUrl1">The first <see cref="DataUrl"/> to compare.</param>
        /// <param name="dataUrl2">The second <see cref="DataUrl"/> to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="dataUrl1"/> and <paramref name="dataUrl2"/> are equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(DataUrl dataUrl1, DataUrl dataUrl2) => dataUrl1.Equals(in dataUrl2);

        /// <summary>
        /// Returns a value that indicates whether the values of two specified <see cref="DataUrl"/> instances are not equal.
        /// </summary>
        /// <param name="dataUrl1">The first <see cref="DataUrl"/> to compare.</param>
        /// <param name="dataUrl2">The second <see cref="DataUrl"/> to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="dataUrl1"/> and <paramref name="dataUrl2"/> are not equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(DataUrl dataUrl1, DataUrl dataUrl2) => !dataUrl1.Equals(in dataUrl2);

        #endregion

        /// <summary>
        /// Determines whether <paramref name="obj"/> is a <see cref="DataUrl"/> structure whose
        /// value is equal to that of this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="DataUrl"/> structure whose
        /// value is equal to that of this instance; <c>false</c>, otherwise.</returns>
        public override bool Equals(object? obj) => obj is DataUrl other && Equals(in other);

        /// <summary>
        /// Creates a hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(GetFileTypeExtension());

            if(TryGetEmbeddedText(out string? text))
            {
                hash.Add(text);
            }
            else if(TryGetEmbeddedBytes(out byte[]? bytes))
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    hash.Add(bytes[i]);
                }
            }
            return hash.ToHashCode();
        }

        /// <summary>
        /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. 
        /// </summary>
        /// <param name="other">The <see cref="DataUrl"/> instance to compare with.</param>
        /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DataUrl other) => Equals(in other);

        /// <summary>
        /// Determines whether the value of this instance is equal to the value of <paramref name="other"/>. 
        /// </summary>
        /// <param name="other">The <see cref="DataUrl"/> instance to compare with.</param>
        /// <returns><c>true</c> if this the value of this instance is equal to that of <paramref name="other"/>; <c>false</c>, otherwise.</returns>
        [CLSCompliant(false)]
        public bool Equals(in DataUrl other)
            => this.IsEmpty || other.IsEmpty
                ? this.IsEmpty && other.IsEmpty
                : EqualsData(in other) && StringComparer.Ordinal.Equals(this.GetFileTypeExtension(), other.GetFileTypeExtension());

        #region private
        private bool EqualsData(in DataUrl other)
            => this.ContainsText
                ? EqualsText(in other)
                : this.DataEncoding == DataEncoding.Base64 && other.DataEncoding == DataEncoding.Base64
                    ? this.EmbeddedData.Equals(other.EmbeddedData, StringComparison.Ordinal)
                    : EqualsBytes(in other);


        private bool EqualsText(in DataUrl other)
        {
            if (other.TryGetEmbeddedText(out string? otherText))
            {
                if (this.TryGetEmbeddedText(out string? thisText))
                {
                    return StringComparer.Ordinal.Equals(thisText, otherText);
                }
            }

            return false;
        }

        private bool EqualsBytes(in DataUrl other)
        {
            if (other.TryGetEmbeddedBytes(out byte[]? otherBytes))
            {
                if (this.TryGetEmbeddedBytes(out byte[]? thisBytes))
                {
                    return thisBytes.SequenceEqual(otherBytes);
                }
            }

            return false;
        }

        #endregion
        #endregion

        #region Parser

        /// <summary>
        /// Parses a <see cref="string"/> as <see cref="DataUrl"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <returns>The <see cref="DataUrl"/> instance, which <paramref name="value"/> represents.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="DataUrl"/>.</exception>
        public static DataUrl Parse(string value)
            => value is null
                ? throw new ArgumentNullException(nameof(value))
                : TryParse(value, out DataUrl dataUrl)
                    ? dataUrl
                    : throw new ArgumentException(string.Format(Res.InvalidDataUrl, nameof(value)), nameof(value));


        /// <summary>
        /// Tries to parse a <see cref="string"/> as <see cref="DataUrl"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <param name="dataUrl">If the method returns <c>true</c> the parameter contains a <see cref="DataUrl"/> structure that provides the contents
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

            for (int i = PROTOCOL.Length; i < value.Length; i++)
            {
                char c = value[i];


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
            ReadOnlySpan<char> mimePart = value.AsSpan(PROTOCOL.Length, mimeTypeEndIndex - PROTOCOL.Length);
            if (HasBase64Encoding(mimePart))
            {
                mimePart = mimePart.Slice(0, mimePart.Length - BASE64.Length);
                mimeTypeEndIndex -= BASE64.Length;
                dataEncoding = DataEncoding.Base64;
            }

            MimeType mediaType;

            if (mimePart.Trim().IsEmpty)
            {
                mediaType = DataUrl.DefaultMediaType();
            }
            else if (!MimeType.TryParse(value[PROTOCOL.Length] == ';'
                ? new StringBuilder(DEFAULT_MEDIA_TYPE.Length + mimePart.Length)
                    .Append(DEFAULT_MEDIA_TYPE)
                    .Append(mimePart).ToString()
                    .AsMemory()
                : value.AsMemory(PROTOCOL.Length, mimeTypeEndIndex - PROTOCOL.Length), out mediaType))
            {
                return false;
            }

            ReadOnlyMemory<char> embeddedData = value.AsMemory(startOfData + 1);
            dataUrl = new DataUrl(in mediaType, dataEncoding, in embeddedData);

            return true;

            //////////////////////////////////////////////////////////////

            static bool HasBase64Encoding(ReadOnlySpan<char> val)
            {
                //Suche ";base64"
                if (val.Length < BASE64.Length)
                {
                    return false;
                }

                ReadOnlySpan<char> hayStack = val.Slice(val.Length - BASE64.Length);

                for (int i = 0; i < hayStack.Length; i++)
                {
                    char c = char.ToLowerInvariant(hayStack[i]);

                    if (c != BASE64[i])
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
            const string charset = ";charset=utf-8";

            if(string.IsNullOrEmpty(text))
            {
                return "data:,";
            }

            text = Uri.UnescapeDataString(text);

            if (text.IsAscii())
            {
                string data = Uri.EscapeDataString(text);
                var sb = new StringBuilder(PROTOCOL.Length + 1 + data.Length);
                return sb.Append(PROTOCOL).Append(',').Append(data).ToString();
            }
            else
            {

                string data = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
                var sb = new StringBuilder(PROTOCOL.Length + charset.Length + BASE64.Length + 1 + data.Length);
                return sb.Append(PROTOCOL).Append(charset).Append(BASE64).Append(',').Append(data).ToString();
            }

            // $"data:,{Uri.EscapeDataString(text)}"
        }

        /// <summary>
        /// Embeds binary data in a "data" URL (RFC 2397).
        /// </summary>
        /// <param name="bytes">The binary data to embed into the "data" URL.</param>
        /// <param name="mediaType">The <see cref="MimeType"/> of the data passed to the parameter <paramref name="bytes"/>.</param>
        /// <returns>A "data" URL, into which the binary data provided by the parameter <paramref name="bytes"/> is embedded.</returns>
        public static string FromBytes(byte[]? bytes, in MimeType mediaType)
        {
            string data = bytes is null ? string.Empty : Convert.ToBase64String(bytes, Base64FormattingOptions.None);
            var builder = new StringBuilder(PROTOCOL.Length + MimeType.StringLength + BASE64.Length + 1 + data.Length);
            return builder.Append(PROTOCOL).AppendMediaType(in mediaType).Append(BASE64).Append(',').Append(data).ToString();

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
        /// <param name="filePath">Abolute path to the file which content is to embed into the "data" URL.</param>
        /// <param name="mimeType">The <see cref="MimeType"/> of the file to embed or <c>null</c> to let the method automatically
        /// retrieve the <see cref="MimeType"/> from the file type extension.</param>
        /// <returns>A "data" URL, into which the content of the file provided by the parameter <paramref name="filePath"/> is embedded.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> is not a valid file path.</exception>
        /// <exception cref="IOException">I/O error.</exception>
        public static string FromFile(string filePath, in MimeType? mimeType = null)
        {
            byte[] bytes = LoadFile(filePath);

            MimeType mimeTypeValue = mimeType ?? MimeType.FromFileTypeExtension(Path.GetExtension(filePath));
            return FromBytes(bytes, in mimeTypeValue);
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
        /// <param name="embeddedBytes">If the method returns <c>true</c> the parameter contains the binary data, which was embedded in the <see cref="DataUrl"/>.
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
        /// Returns an appropriate file type extension for the data embedded in the <see cref="DataUrl"/>. The file type extension contains the 
        /// period (".").
        /// </summary>
        /// <returns>An appropriate file type extension for the data embedded in the <see cref="DataUrl"/>.</returns>
        ///// <remarks>The search for a file type extension can be an expensive operation. To make subsequent calls of the method faster, the
        ///// recent file type extensions are stored in a cache. You can call <see cref="MimeType.ClearCache"/> to clear this cache.</remarks>
        public string GetFileTypeExtension() => MimeType.GetFileTypeExtension();

        #endregion

        [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal static MimeType DefaultMediaType()
        {
            ReadOnlyMemory<char> memory = DEFAULT_MEDIA_TYPE.AsMemory();
            _ = MimeType.TryParse(in memory, out MimeType mediaType);
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
