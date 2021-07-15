using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;
using FolkerKinzel.Uris.Properties;

#if NETSTANDARD2_0
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    /// <summary>
    /// Repräsentiert einen Data-URL nach RFC 2397.
    /// </summary>
    public static class DataUrlBuilder
    {
        private const int PROTOCOL_LENGTH = 5;
        private const int BASE64_LENGTH = 8;


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

            return DataUrlBuilder.FromBytes(bytes, mediaType.Value);
        }


        public static string FromFile(string path, InternetMediaType? mediaType = null)
        {
            byte[] bytes = LoadFile(path);

            if (mediaType is null)
            {
                mediaType = InternetMediaType.FromFileTypeExtension(Path.GetExtension(path));
            }

            return DataUrlBuilder.FromBytes(bytes, mediaType.Value);
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
