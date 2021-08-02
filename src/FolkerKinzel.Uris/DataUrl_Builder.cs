﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrl : IEquatable<DataUrl>, ICloneable
    {
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

            if (string.IsNullOrEmpty(text))
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
        /// <param name="mimeType">The <see cref="MimeType"/> of the data passed to the parameter <paramref name="bytes"/>.</param>
        /// <returns>A "data" URL, into which the binary data provided by the parameter <paramref name="bytes"/> is embedded.</returns>
        public static string FromBytes(byte[]? bytes, in MimeType mimeType)
        {
            string data = bytes is null ? string.Empty : Convert.ToBase64String(bytes, Base64FormattingOptions.None);
            var builder = new StringBuilder(PROTOCOL.Length + MimeType.StringLength + BASE64.Length + 1 + data.Length);
            return builder.Append(PROTOCOL).AppendMediaType(in mimeType).Append(BASE64).Append(',').Append(data).ToString();

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
        #endregion


    }
}