using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes;
using FolkerKinzel.Strings;
using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrlInfo
    {

        /// <summary>
        /// Tries to retrieve the text, which is embedded in the "data" URL.
        /// </summary>
        /// <param name="embeddedText">If the method returns <c>true</c> the parameter contains the text, which was embedded in the <see cref="DataUrlInfo"/>.
        /// The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the data embedded in the data url could be parsed as text, <c>false</c> otherwise.</returns>
        public bool TryGetEmbeddedText([NotNullWhen(true)] out string? embeddedText)
        {
            embeddedText = null;
            if (!ContainsEmbeddedText)
            {
                return false;
            }

            // als Base64 codierter Text:
            if (Encoding == ContentEncoding.Base64)
            {
                static bool Predicate(MimeTypeParameter p) => p.IsCharsetParameter;

                MimeTypeParameter charsetParameter = MimeType.Parameters.FirstOrDefault(Predicate);

                byte[] data;
                try
                {
                    data = Convert.FromBase64String(Data.ToString());
                }
                catch
                {
                    return false;
                }

                int codePage = TextEncodingConverter.GetCodePage(data, out int bomLength);

                Encoding enc = charsetParameter.IsEmpty
                                ? TextEncodingConverter.GetEncoding(codePage)
                                : charsetParameter.IsAsciiCharsetParameter
                                    ? System.Text.Encoding.UTF8
                                    : TextEncodingConverter.GetEncoding(charsetParameter.Value.ToString());

                try
                {
                    embeddedText = enc.GetString(data, bomLength, data.Length - bomLength);
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                // Url-Codierter UTF-8-String:
                embeddedText = Uri.UnescapeDataString(Data.ToString());
            }

            return true;
        }



        /// <summary>
        /// Tries to retrieve the binary data, which is embedded in the "data" URL.
        /// </summary>
        /// <param name="embeddedBytes">If the method returns <c>true</c> the parameter contains the binary data, which was embedded in the <see cref="DataUrlInfo"/>.
        /// The parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the data embedded in the data url could be parsed as binary data, <c>false</c> otherwise.</returns>
        public bool TryGetEmbeddedBytes([NotNullWhen(true)] out byte[]? embeddedBytes)
        {
            embeddedBytes = null;

            if (!ContainsEmbeddedBytes)
            {
                return false;
            }

            try
            {
                if (this.Encoding == ContentEncoding.Base64)
                {
                    embeddedBytes = Convert.FromBase64String(Data.ToString());
                }
                else
                {
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Data.ToString());
                    embeddedBytes = WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Returns an appropriate file type extension for the data embedded in the "data" URL. The file type extension contains the 
        /// period (".").
        /// </summary>
        /// <returns>An appropriate file type extension for the data embedded in the <see cref="DataUrlInfo"/>.</returns>
        ///// <remarks>The search for a file type extension can be an expensive operation. To make subsequent calls of the method faster, the
        ///// recent file type extensions are stored in a cache. You can call <see cref="MimeType.ClearCache"/> to clear this cache.</remarks>
        public string GetFileTypeExtension() => MimeType.GetFileTypeExtension();



    }
}
