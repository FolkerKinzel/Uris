using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris
{
    /// <summary>
    /// Kapselt die in einem Data-URL (RFC 2397) enthaltenen Informationen.
    /// </summary>
    public class DataUrlInfo
    {
        internal DataUrlInfo(InternetMediaType mediaType, DataEncoding dataEncoding, string embeddedData)
        {
            MediaType = mediaType;
            DataEncoding = dataEncoding;
            EmbeddedData = embeddedData;
        }

        /// <summary>
        /// Der Datentyp der im Data-URL eingebetteten Daten.
        /// </summary>
        public InternetMediaType MediaType { get; }

        /// <summary>
        /// Die Art der Enkodierung der in <see cref="EmbeddedData"/> enthaltenen Daten.
        /// </summary>
        public DataEncoding DataEncoding { get; }

        /// <summary>
        /// Der Teil des Data-URLs, der die eingebetteten Daten enthält.
        /// </summary>
        public string EmbeddedData { get; }


        /// <summary>
        /// <c>true</c>, wenn der Data-Url eingebetteten Text enthält.
        /// </summary>
        public bool ContainsText => this.MediaType.MediaType.Equals("text", StringComparison.Ordinal);


        /// <summary>
        /// <c>true</c>, wenn der Data-URL eingebettete binäre Daten enthält.
        /// </summary>
        public bool ContainsBytes => DataEncoding == DataEncoding.Base64 || !ContainsText;


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
        /// zurück. Die Dateiendung enthält den Punkt "." als Trennzeichen.
        /// </summary>
        /// <param name="cacheLifeTime">Die Lebensdauer des angelegten Caches in Minuten.</param>
        /// <returns>Ein <see cref="Task{TResult}"/>-Objekt, das den Zugriff auf eine geeignete Dateiendung für die in den Data-URL
        /// eingebetteten Daten ermöglicht.</returns>
        /// <remarks>Da das Auffinden einer geeigneten Dateiendung ein aufwändiger Vorgang ist, werden Suchergebnisse für eine
        /// kurze Zeitspanne in einem Cache zwischengespeichert, um die Performance zu erhöhen.</remarks>
        public Task<string> GetFileTypeExtensionAsync(double cacheLifeTime = 5) => MediaType.GetFileTypeExtensionAsync(cacheLifeTime);
    }
}
