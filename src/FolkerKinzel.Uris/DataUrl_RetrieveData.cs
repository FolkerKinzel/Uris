using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrl : IEquatable<DataUrl>, ICloneable
    {
        #region Retrieve Data

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
                static bool Predicate(MimeTypeParameter p) => p.IsCharsetParameter;

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
                if(this.DataEncoding == DataEncoding.Base64)
                {
                    embeddedBytes = Convert.FromBase64String(EmbeddedData.ToString());
                }
                else
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(EmbeddedData.ToString());
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
        /// Returns an appropriate file type extension for the data embedded in the <see cref="DataUrl"/>. The file type extension contains the 
        /// period (".").
        /// </summary>
        /// <returns>An appropriate file type extension for the data embedded in the <see cref="DataUrl"/>.</returns>
        ///// <remarks>The search for a file type extension can be an expensive operation. To make subsequent calls of the method faster, the
        ///// recent file type extensions are stored in a cache. You can call <see cref="MimeType.ClearCache"/> to clear this cache.</remarks>
        public string GetFileTypeExtension() => MimeType.GetFileTypeExtension();

        #endregion


    }
}
