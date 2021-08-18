using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes;
using FolkerKinzel.Strings;
using FolkerKinzel.Strings.Polyfills;
using FolkerKinzel.Uris.Extensions;
using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris
{
    /// <summary>
    /// Static class, which provides methods to support the work with <see cref="string"/>s and <see cref="Uri"/>s
    /// that represent a "data" URL (RFC 2397) that embeds data in-line in a URL.
    /// </summary>
    public static class DataUrl
    {
        #region const
        internal const string Protocol = "data:";
        internal const string Base64 = ";base64";
        private const string DEFAULT_MEDIA_TYPE = "text/plain";

        #endregion
        

        /// <summary>
        /// Embeds Text in a "data" URL (RFC 2397).
        /// </summary>
        /// <param name="text">The text to embed into the "data" URL. <paramref name="text"/> MUST not be URL encoded.</param>
        /// <returns>A "data" URL, into which the text provided by the parameter <paramref name="text"/> is embedded.</returns>
        /// <exception cref="FormatException">The <see cref="Uri"/> class was not able to encode <paramref name="text"/> correctly.</exception>
        public static string BuildFromEmbeddedText(string? text)
        {
            const string charset = ";charset=utf-8";

            if (string.IsNullOrEmpty(text))
            {
                return "data:,";
            }

            text = Uri.UnescapeDataString(text);

            if (text.IsAscii())
            {
                string data = Uri.EscapeDataString(text);
                var sb = new StringBuilder(Protocol.Length + 1 + data.Length);
                return sb.Append(Protocol).Append(',').Append(data).ToString();
            }
            else
            {

                string data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
                var sb = new StringBuilder(Protocol.Length + charset.Length + Base64.Length + 1 + data.Length);
                return sb.Append(Protocol).Append(charset).Append(Base64).Append(',').Append(data).ToString();
            }

            // $"data:,{Uri.EscapeDataString(text)}"
        }

        /// <summary>
        /// Embeds binary data in a "data" URL (RFC 2397).
        /// </summary>
        /// <param name="bytes">The binary data to embed into the "data" URL.</param>
        /// <param name="mimeType">The <see cref="MimeType"/> of the data passed to the parameter <paramref name="bytes"/>.</param>
        /// <returns>A "data" URL, into which the binary data provided by the parameter <paramref name="bytes"/> is embedded.</returns>
        public static string BuildFromEmbeddedBytes(byte[]? bytes, in MimeType mimeType)
        {
            string data = bytes is null ? string.Empty : Convert.ToBase64String(bytes, Base64FormattingOptions.None);
            var builder = new StringBuilder(Protocol.Length + FolkerKinzel.MimeTypes.MimeType.StringLength + Base64.Length + 1 + data.Length);
            return builder.Append(Protocol).AppendMediaType(in mimeType).Append(Base64).Append(',').Append(data).ToString();

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
        public static string BuildFromEmbeddedFileContent(string filePath, in MimeType? mimeType = null)
        {
            byte[] bytes = LoadFile(filePath);

            MimeType mimeTypeValue = mimeType ?? MimeTypes.MimeType.FromFileTypeExtension(Path.GetExtension(filePath));
            return BuildFromEmbeddedBytes(bytes, in mimeTypeValue);
        }


        
        #region Parser

        ///// <summary>
        ///// Parses a <see cref="string"/> as <see cref="DataUrlInfo"/>.
        ///// </summary>
        ///// <param name="value">The <see cref="string"/> to parse.</param>
        ///// <returns>The <see cref="DataUrlInfo"/> instance, which <paramref name="value"/> represents.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        ///// <exception cref="ArgumentException"><paramref name="value"/> value could not be parsed as <see cref="DataUrlInfo"/>.</exception>
        //public static DataUrlInfo Parse(string value)
        //    => value is null
        //        ? throw new ArgumentNullException(nameof(value))
        //        : TryParse(value, out DataUrlInfo dataUrl)
        //            ? dataUrl
        //            : throw new ArgumentException(string.Format(Res.InvalidDataUrl, nameof(value)), nameof(value));


        /// <summary>
        /// Tries to parse a <see cref="string"/> as <see cref="DataUrlInfo"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> to parse.</param>
        /// <param name="info">If the method returns <c>true</c> the parameter contains a <see cref="DataUrlInfo"/> structure 
        /// that provides the contents
        /// of <paramref name="value"/>, otherwise <c>null</c>. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrlInfo"/>, otherwise <c>false</c>.</returns>
        public static bool TryParse(string? value, [NotNullWhen(true)] out DataUrlInfo? info)
        {
            ReadOnlyMemory<char> memory = value.AsMemory();
            return TryParse(in memory, out info);
        }

        /// <summary>
        /// Tries to parse a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> as <see cref="DataUrlInfo"/>.
        /// </summary>
        /// <param name="value">The <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> to parse.</param>
        /// <param name="info">If the method returns <c>true</c> the parameter contains a <see cref="DataUrlInfo"/> 
        /// structure that provides the contents
        /// of <paramref name="value"/>, otherwise <c>null</c>. The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if <paramref name="value"/> could be parsed as <see cref="DataUrlInfo"/>, <c>false</c> otherwise.</returns>
        public static bool TryParse(in ReadOnlyMemory<char> value, [NotNullWhen(true)] out DataUrlInfo? info)
        {
            ReadOnlySpan<char> span = value.Span;

            if (!span.IsDataUrl())
            {
                goto Failed;
            }

            int mimeTypeEndIndex = -1;
            int startOfData = -1;

            for (int i = DataUrl.Protocol.Length; i < span.Length; i++)
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
                goto Failed;
            }

            // dies ändert ggf. auch mimeTypeEndIndex
            ReadOnlySpan<char> mimePart = span.Slice(DataUrl.Protocol.Length, mimeTypeEndIndex - DataUrl.Protocol.Length);
            ContentEncoding dataEncoding = ContentEncoding.Url;

            if (HasBase64Encoding(mimePart))
            {
                mimePart = mimePart.Slice(0, mimePart.Length - DataUrl.Base64.Length);
                mimeTypeEndIndex -= DataUrl.Base64.Length;
                dataEncoding = ContentEncoding.Base64;
            }

            MimeType mediaType;

            if (mimePart.IsEmpty)
            {
                mediaType = DataUrl.DefaultMediaType();
            }
            else
            {
                ReadOnlyMemory<char> memory = span[DataUrl.Protocol.Length] == ';'
                                                ? new StringBuilder(DEFAULT_MEDIA_TYPE.Length + mimePart.Length)
                                                    .Append(DEFAULT_MEDIA_TYPE)
                                                    .Append(mimePart).ToString()
                                                    .AsMemory()
                                                : value.Slice(DataUrl.Protocol.Length, mimeTypeEndIndex - DataUrl.Protocol.Length);

                if (!MimeType.TryParse(ref memory, out mediaType))
                {
                    goto Failed;
                }
            }
            ReadOnlyMemory<char> embeddedData = value.Slice(startOfData + 1);
            info = new DataUrlInfo(in mediaType, dataEncoding, in embeddedData);

            return true;

Failed:
            info = null;
            return false;


            //////////////////////////////////////////////////////////////

            static bool HasBase64Encoding(ReadOnlySpan<char> val)
            {
                //Suche ";base64"
                if (val.Length < DataUrl.Base64.Length)
                {
                    return false;
                }

                ReadOnlySpan<char> hayStack = val.Slice(val.Length - DataUrl.Base64.Length);

                for (int i = 0; i < hayStack.Length; i++)
                {
                    char c = char.ToLowerInvariant(hayStack[i]);

                    if (c != DataUrl.Base64[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal static MimeType DefaultMediaType()
        {
            ReadOnlyMemory<char> memory = DEFAULT_MEDIA_TYPE.AsMemory();
            _ = MimeType.TryParse(ref memory, out MimeType mediaType);
            return mediaType;
        }

        #endregion

        
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
