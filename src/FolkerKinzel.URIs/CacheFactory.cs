using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.URIs
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class CacheFactory
    {
        internal static Dictionary<string, string> CreateMimeTypeCache()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            { 
                {"application/json", ".json" },
                {"application/msword", ".doc" },
                {"application/pdf", ".pdf" },
                {"application/rtf", ".rtf" },
                {"application/vnd.ms-excel", ".xls" },
                {"application/vnd.ms-powerpoint", ".ppt" },
                {"application/vnd.oasis.opendocument.presentation", ".odp" },
                {"application/vnd.oasis.opendocument.spreadsheet", ".ods" },
                {"application/vnd.oasis.opendocument.text", ".odt" },
                {"application/vnd.openxmlformats-officedocument.presentationml.presentation", ".pptx" },
                {"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx" },
                {"application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx" },
                {"application/xml", ".xml" },
                {"application/zip", ".zip" },
                {"audio/mpeg", ".mp3" },
                {"image/gif", ".gif" },
                {"image/jpeg", ".jpg" },
                {"image/png", ".png" },
                {"image/svg+xml", ".svg" },
                {"image/x-icon", ".ico" },
                {"message/rfc822", ".eml" },
                {"text/csv", ".csv" },
                {"text/html", ".htm"},
                {"text/plain", ".txt" },
                {"text/plain", ".log" },
                {"text/x-vcard", ".vcf" }
            };
        }


        public static Dictionary<string, string> CreateFyleTypeCache()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {".json", "application/json" },
                {".doc","application/msword" },
                {".pdf","application/pdf" },
                {".rtf","application/rtf" },
                {".xls","application/vnd.ms-excel" },
                {".ppt","application/vnd.ms-powerpoint" },
                {".odp", "application/vnd.oasis.opendocument.presentation" },
                {".ods", "application/vnd.oasis.opendocument.spreadsheet" },
                {".odt", "application/vnd.oasis.opendocument.text" },
                {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                {".xml", "application/xml" },
                {".zip", "application/zip" },
                {".mp3", "audio/mpeg" },
                {".gif", "image/gif" },
                {".jpeg", "image/jpeg" },
                {".jpg", "image/jpeg" },
                {".png", "image/png" },
                {".svg", "image/svg+xml" },
                {".ico", "image/x-icon" },
                {".eml", "message/rfc822" },
                {".csv", "text/csv" },
                {".html", "text/html" },
                {".htm", "text/html" },
                {".txt", "text/plain" },
                {".log", "text/plain" },
                {".vcf", "text/x-vcard" }
            };
        }
    }
}
