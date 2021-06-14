using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LazyCache;

namespace FolkerKinzel.URIs.Intls
{
    internal static class MimeCache
    {
        private static readonly IAppCache cache = new CachingService();
        private const string RESOURCE_NAME = "FolkerKinzel.MimeTypes.Resources.Mime.csv";
        private const string DEFAULT_MIME_TYPE = "application/octet-stream";
        private const string DEFAULT_FILE_TYPE_EXTENSION = "bin";

        internal static string GetMimeType(string fileTypeExtension)
        {
            Dictionary<string, string> dic = cache.GetOrAdd("mime", CreateMimeTypeCache, DateTimeOffset.Now + TimeSpan.FromMinutes(10));

            if(dic.ContainsKey(fileTypeExtension))
            {
                return dic[fileTypeExtension];
            }

            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_NAME);

            if(stream is null)
            {
                return DEFAULT_MIME_TYPE;
            }

            using var reader = new StreamReader(stream);

            string? line;
            while((line = reader.ReadLine()) is not null)
            {
                int separatorIndex = line.IndexOf(' ');

                ReadOnlySpan<char> span = line.AsSpan(separatorIndex + 1);

                if(span.Equals(fileTypeExtension, StringComparison.OrdinalIgnoreCase))
                {
                    string mime = line.Substring(0, separatorIndex);
                    dic[fileTypeExtension] = mime;

                    return mime;
                }
            }

            return DEFAULT_MIME_TYPE;
        }


        internal static string GetFileTypeExtension(string mimeType)
        {
            Dictionary<string, string> dic = cache.GetOrAdd("extensions", CreateFyleTypeCache, DateTimeOffset.Now + TimeSpan.FromMinutes(10));

            if(dic.ContainsKey(mimeType))
            {
                return dic[mimeType];
            }

            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_NAME);

            if(stream is null)
            {
                return DEFAULT_MIME_TYPE;
            }

            using var reader = new StreamReader(stream);

            string? line;
            while((line = reader.ReadLine()) is not null)
            {
                int separatorIndex = line.IndexOf(' ');

                ReadOnlySpan<char> span = line.AsSpan(0, separatorIndex);

                if(span.Equals(mimeType, StringComparison.OrdinalIgnoreCase))
                {
                    string fileType = line.Substring(separatorIndex + 1);
                    dic[mimeType] = fileType;

                    return fileType;
                }
            }

            return DEFAULT_FILE_TYPE_EXTENSION;
        }


        private static Dictionary<string, string> CreateMimeTypeCache()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }


        private static Dictionary<string, string> CreateFyleTypeCache()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {".json", "application/json" },
                {".doc","application/msword" },
                {".pdf","application/pdf" },
                {".pgp","application/pgp-encrypted" },
                {".sig","application/pgp-signature" },
                {".ai","application/postscript" },
                {".ps","application/postscript" },
                {".rtf","application/rtf" },
                {".xls","application/vnd.ms-excel" },
                {".chm","application/vnd.ms-htmlhelp" },
                {".ppt","application/vnd.ms-powerpoint" },
                {".xps", "application/vnd.ms-xpsdocument" }
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };
                //{"", "" };

            };
        }
    }
}
