using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace FolkerKinzel.URIs
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CacheFactory
    {
        internal static ConcurrentDictionary<string, string> CreateFyleTypeCache()
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
    }
}
