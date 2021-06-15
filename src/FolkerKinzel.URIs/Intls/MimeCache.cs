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
        private const string MIME_CACHE_NAME = "mime";
        private const string EXTENSION_CACHE_NAME = "extensions";

        internal static string GetMimeType(string fileTypeExtension, double cacheLifeTime)
        {
            Dictionary<string, string> dic = cache.GetOrAdd(MIME_CACHE_NAME, CacheProvider.CreateMimeTypeCache, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(cacheLifeTime));

            if (dic.ContainsKey(fileTypeExtension))
            {
                return dic[fileTypeExtension];
            }

            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_NAME);

            if (stream is null)
            {
                return DEFAULT_MIME_TYPE;
            }

            using var reader = new StreamReader(stream);

            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                int separatorIndex = line.IndexOf(' ');

                ReadOnlySpan<char> span = line.AsSpan(separatorIndex + 1);


                if (span.Equals(fileTypeExtension.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    string mime = line.Substring(0, separatorIndex);
                    dic[fileTypeExtension] = mime;

                    return mime;
                }
            }

            return DEFAULT_MIME_TYPE;
        }


        internal static string GetFileTypeExtension(string mimeType, double cacheLifeTime)
        {
            Dictionary<string, string> dic = cache.GetOrAdd(EXTENSION_CACHE_NAME, CacheProvider.CreateFyleTypeCache, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(cacheLifeTime));

            if (dic.ContainsKey(mimeType))
            {
                return dic[mimeType];
            }

            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_NAME);

            if (stream is null)
            {
                return DEFAULT_MIME_TYPE;
            }

            using var reader = new StreamReader(stream);

            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                int separatorIndex = line.IndexOf(' ');

                ReadOnlySpan<char> span = line.AsSpan(0, separatorIndex);

                if (span.Equals(mimeType.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    string fileType = line.Substring(separatorIndex + 1);
                    dic[mimeType] = fileType;

                    return fileType;
                }
            }

            return DEFAULT_FILE_TYPE_EXTENSION;
        }


        private static DateTimeOffset ComputeExpirationTime(double cacheLifeTime)
            => DateTimeOffset.UtcNow + TimeSpan.FromMinutes(cacheLifeTime);
    }
}
