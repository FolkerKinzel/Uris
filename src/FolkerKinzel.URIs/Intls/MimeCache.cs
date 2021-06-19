using System;
using System.Collections.Concurrent;
using LazyCache;

namespace FolkerKinzel.URIs.Intls
{
    internal static class MimeCache
    {
        private static readonly IAppCache cache = new CachingService();

        private const string MIME_CACHE_NAME = "mime";
        private const string EXTENSION_CACHE_NAME = "extensions";


        internal static string GetMimeType(string fileTypeExtension, double cacheLifeTime)
        {
            ConcurrentDictionary<string, string> dic = cache.GetOrAdd(MIME_CACHE_NAME, CacheFactory.CreateMimeTypeCache, ComputeExpirationTime(cacheLifeTime));

            if (dic.TryGetValue(fileTypeExtension, out string? result))
            {
                return result;
            }

            result = ResourceParser.GetMimeType(fileTypeExtension);
            _ = dic.TryAdd(fileTypeExtension, result);
            return result;
        }



        internal static string GetFileTypeExtension(string mimeType, double cacheLifeTime)
        {
            ConcurrentDictionary<string, string> dic = cache.GetOrAdd(EXTENSION_CACHE_NAME, CacheFactory.CreateFileTypeCache, ComputeExpirationTime(cacheLifeTime));

            if (dic.TryGetValue(mimeType, out string? result))
            {
                return result;
            }

            result = ResourceParser.GetFileType(mimeType);
            _ = dic.TryAdd(mimeType, result);
            return result;
        }


        private static DateTimeOffset ComputeExpirationTime(double cacheLifeTime)
            => DateTimeOffset.UtcNow + TimeSpan.FromMinutes(cacheLifeTime);
    }
}
