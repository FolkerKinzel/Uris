using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace FolkerKinzel.URIs
{
    /// <summary>
    /// Klasse, die die mit häufig vorkommenden Datentypen gefüllten Dictionaries erstellt, die als Cache Verwendung finden.
    /// </summary>
    /// <remarks>
    /// Die Klasse ist public, da sie von MimeResourceCompiler verwendet wird.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CacheFactory
    {
        /// <summary>
        /// Erzeugt ein vorgefülltes Dictionary, das als Datenquelle für den Cache dient, der verwendet wird, um eine Dateiendung
        /// für einen MIME-Typ zu finden.
        /// </summary>
        /// <returns>Ein vorgefülltes Dictionary, das als Datenquelle für den Cache dient, der verwendet wird, um eine Dateiendung
        /// für einen MIME-Typ zu finden.</returns>
        internal static ConcurrentDictionary<string, string> CreateFileTypeCache()
        {
            var dic = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _ = dic.TryAdd("application/json", ".json");
            _ = dic.TryAdd("application/msword", ".doc");
            _ = dic.TryAdd("application/pdf", ".pdf");
            _ = dic.TryAdd("application/rtf", ".rtf");
            _ = dic.TryAdd("application/vnd.ms-excel", ".xls");
            _ = dic.TryAdd("application/vnd.ms-powerpoint", ".ppt");
            _ = dic.TryAdd("application/vnd.oasis.opendocument.presentation", ".odp");
            _ = dic.TryAdd("application/vnd.oasis.opendocument.spreadsheet", ".ods");
            _ = dic.TryAdd("application/vnd.oasis.opendocument.text", ".odt");
            _ = dic.TryAdd("application/vnd.openxmlformats-officedocument.presentationml.presentation", ".pptx");
            _ = dic.TryAdd("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx");
            _ = dic.TryAdd("application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx");
            _ = dic.TryAdd("application/xml", ".xml");
            _ = dic.TryAdd("application/zip", ".zip");
            _ = dic.TryAdd("audio/mpeg", ".mp3");
            _ = dic.TryAdd("image/gif", ".gif");
            _ = dic.TryAdd("image/jpeg", ".jpg");
            _ = dic.TryAdd("image/png", ".png");
            _ = dic.TryAdd("image/svg+xml", ".svg");
            _ = dic.TryAdd("image/x-icon", ".ico");
            _ = dic.TryAdd("message/rfc822", ".eml");
            _ = dic.TryAdd("text/csv", ".csv");
            _ = dic.TryAdd("text/html", ".htm");
            _ = dic.TryAdd("text/plain", ".txt");
            _ = dic.TryAdd("text/plain", ".log");
            _ = dic.TryAdd("text/x-vcard", ".vcf");

            return dic;
        }

        /// <summary>
        /// Erzeugt ein vorgefülltes Dictionary, das als Datenquelle für den Cache dient, der verwendet wird, um einen MIME-Typ für eine Dateiendung
        /// zu finden.
        /// </summary>
        /// <returns>Ein vorgefülltes Dictionary, das als Datenquelle für den Cache dient, der verwendet wird, um einen MIME-Typ für eine Dateiendung
        /// zu finden.</returns>
        /// <remarks>Die Methode ist public, weil sie von MimeResourceCompiler aufgerufen wird.</remarks>
        public static ConcurrentDictionary<string, string> CreateMimeTypeCache()
        {
            var dic = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _ = dic.TryAdd(".json", "application/json");
            _ = dic.TryAdd(".doc", "application/msword");
            _ = dic.TryAdd(".pdf", "application/pdf");
            _ = dic.TryAdd(".rtf", "application/rtf");
            _ = dic.TryAdd(".xls", "application/vnd.ms-excel");
            _ = dic.TryAdd(".ppt", "application/vnd.ms-powerpoint");
            _ = dic.TryAdd(".odp", "application/vnd.oasis.opendocument.presentation");
            _ = dic.TryAdd(".ods", "application/vnd.oasis.opendocument.spreadsheet");
            _ = dic.TryAdd(".odt", "application/vnd.oasis.opendocument.text");
            _ = dic.TryAdd(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation");
            _ = dic.TryAdd(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            _ = dic.TryAdd(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            _ = dic.TryAdd(".xml", "application/xml");
            _ = dic.TryAdd(".zip", "application/zip");
            _ = dic.TryAdd(".mp3", "audio/mpeg");
            _ = dic.TryAdd(".gif", "image/gif");
            _ = dic.TryAdd(".jpeg", "image/jpeg");
            _ = dic.TryAdd(".jpg", "image/jpeg");
            _ = dic.TryAdd(".png", "image/png");
            _ = dic.TryAdd(".svg", "image/svg+xml");
            _ = dic.TryAdd(".ico", "image/x-icon");
            _ = dic.TryAdd(".eml", "message/rfc822");
            _ = dic.TryAdd(".csv", "text/csv");
            _ = dic.TryAdd(".html", "text/html");
            _ = dic.TryAdd(".htm", "text/html");
            _ = dic.TryAdd(".txt", "text/plain");
            _ = dic.TryAdd(".log", "text/plain");
            _ = dic.TryAdd(".vcf", "text/x-vcard");

            return dic;
        }


        public static void TestIt()
        {
            ConcurrentDictionary<string, string> fileTypeCache = CreateFileTypeCache();

            string? error = fileTypeCache.Select(kvp => $"{kvp.Key} {kvp.Value}").FirstOrDefault(x => x.Any(c => char.IsUpper(c)));

            if(error is not null)
            {
                throw new InvalidDataException($"{nameof(FolkerKinzel.URIs)}.{nameof(CacheFactory)}: File type cache contains an uppercase letter at \"{error}\".");
            }

            error = fileTypeCache.FirstOrDefault(kvp => string.IsNullOrEmpty(kvp.Value) || kvp.Value.IndexOf(' ') != -1 || kvp.Value.IndexOf('.') != 0).Value;

            if(error is not null)
            {
                throw new InvalidDataException($"{nameof(FolkerKinzel.URIs)}.{nameof(CacheFactory)}: File type cache contains an invalid value at \"{error}\".");
            }

            error = fileTypeCache.FirstOrDefault(kvp => kvp.Key.IndexOf(' ') != -1).Key;

            if(error is not null)
            {
                throw new InvalidDataException($"{nameof(FolkerKinzel.URIs)}.{nameof(CacheFactory)}: File type cache contains an invalid key at \"{error}\".");
            }


            ConcurrentDictionary<string, string> mimeTypeCache = CreateMimeTypeCache();

            error = mimeTypeCache.Select(kvp => $"{kvp.Key} {kvp.Value}").FirstOrDefault(x => x.Any(c => char.IsUpper(c)));

            if(error is not  null)
            {
                throw new InvalidDataException($"{nameof(FolkerKinzel.URIs)}.{nameof(CacheFactory)}: Mime type cache contains an uppercase letter at \"{error}\"");
            }

            error = mimeTypeCache.FirstOrDefault(kvp => string.IsNullOrEmpty(kvp.Value) || kvp.Value.IndexOf(' ') != -1).Value;

            if(error is not null)
            {
                throw new InvalidDataException($"{nameof(FolkerKinzel.URIs)}.{nameof(CacheFactory)}: Mime type cache contains an invalid value at \"{error}\".");
            }


            error = mimeTypeCache.FirstOrDefault(kvp => kvp.Key.IndexOf(' ') != -1 || kvp.Key.IndexOf('.') != 0).Key;

            if(error is not null)
            {
                throw new InvalidDataException($"{nameof(FolkerKinzel.URIs)}.{nameof(CacheFactory)}: File type cache contains an invalid key at \"{error}\".");
            }

            error = mimeTypeCache.Values.Distinct(StringComparer.Ordinal).FirstOrDefault(s => !fileTypeCache.ContainsKey(s));

            if (error is not null)
            {
                throw new InvalidDataException($"{nameof(FolkerKinzel)}.{nameof(URIs)}.{nameof(CacheFactory)}: Mime type cache contains the value \"{error}\", which is not a key in file type cache.");
            }

        }
    }
}
