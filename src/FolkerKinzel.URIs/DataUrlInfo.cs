using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.URIs.Intls;

namespace FolkerKinzel.URIs
{
    public class DataUrlInfo
    {
        internal DataUrlInfo(InternetMediaType mediaType, DataEncoding dataEncoding, string embeddedData)
        {
            MediaType = mediaType;
            DataEncoding = dataEncoding;
            EmbeddedData = embeddedData;
        }

        public InternetMediaType MediaType { get; }

        public DataEncoding DataEncoding { get; }

        public string EmbeddedData { get; }


        /// <summary>
        /// <c>true</c>, wenn der Data-Url eingebetteten Text enthält.
        /// </summary>
        public bool ContainsText => this.MediaType.MediaType.Equals("text", StringComparison.Ordinal);


        /// <summary>
        /// <c>true</c>, wenn der Data-Url eingebettete binäre Daten enthält.
        /// </summary>
        public bool ContainsBytes => !ContainsText;


        /// <summary>
        /// Gibt den im Data-Url eingebetteten Text zurück oder <c>null</c>,
        /// wenn der Data-Url eingebettete binäre Daten enthält.
        /// </summary>
        /// <returns>Der eingebettete Text oder <c>null</c>.</returns>
        public bool TryGetEmbeddedText([NotNullWhen(true)] out string? embeddedText)
        {
            embeddedText = null;
            if (!ContainsText)
            {
                return false;
            }

            if (DataEncoding == DataEncoding.Base64)
            {
                // als Base64 codierter Text:
                Encoding enc = MediaType.Parameters.ContainsKey(InternetMediaType.CHARSET_PARAMETER_NAME)
                    ? TextEncodingConverter.GetEncoding(MediaType.Parameters[InternetMediaType.CHARSET_PARAMETER_NAME])
                    : Encoding.ASCII;

                try
                {
                    embeddedText = enc.GetString(Convert.FromBase64String(EmbeddedData));
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                // Url-Codierter UTF-8-String:
                embeddedText = Uri.UnescapeDataString(EmbeddedData);
            }

            return true;
        }


        /// <summary>
        /// Gibt die im Data-Url eingebetteten binären Daten zurück oder <c>null</c>,
        /// wenn der Data-Url keine eingebetteten binären Daten enthält oder wenn
        /// diese nicht dekodiert werden konnten.
        /// </summary>
        /// <returns>Die eingebetteten binären Daten oder <c>null</c>.</returns>
        public bool TryGetEmbeddedBytes([NotNullWhen(true)] out byte[]? embeddedBytes)
        {
            embeddedBytes = null;

            if(!ContainsBytes)
            {
                return false;
            }

            try
            {
                embeddedBytes = this.DataEncoding == DataEncoding.Base64
                    ? Convert.FromBase64String(EmbeddedData)
                    : System.Text.Encoding.UTF8.GetBytes(Uri.UnescapeDataString(EmbeddedData));
            }
            catch
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Gibt eine geeignete Dateiendung für die in den den Data-Url eingebetteten Daten 
        /// zurück.
        /// </summary>
        /// <returns>Eine geeignete Dateiendung für die in den Data-Url
        /// eingebetteten Daten. Die Dateiendung enthält den Punkt "." als Trennzeichen.</returns>
        public Task<string> GetFileTypeExtensionAsync(double cacheLifeTime = 5) => MediaType.GetFileTypeExtensionAsync(cacheLifeTime);
    }
}
