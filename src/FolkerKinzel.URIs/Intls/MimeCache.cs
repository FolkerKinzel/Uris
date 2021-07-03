using System;
using System.Collections.Concurrent;
using LazyCache;

#if NETSTANDARD2_0
using FolkerKinzel.Strings;
#endif

namespace FolkerKinzel.Uris.Intls
{
    /// <summary>
    /// Cache, zum Finden von Dateiendungen für MIME-Typen und für das Finden passender MIME-Typen für Dateiendungen.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    internal static class MimeCache
    {
        private static readonly IAppCache _cache = new CachingService();

        private const string MIME_CACHE_NAME = "mime";
        private const string EXTENSION_CACHE_NAME = "extensions";


        internal static string GetMimeType(string fileTypeExtension, double cacheLifeTime)
        {
            fileTypeExtension = fileTypeExtension.Replace(".", null).Replace(" ", null).ToLowerInvariant();

            ConcurrentDictionary<string, string> dic = _cache.GetOrAdd(MIME_CACHE_NAME, CacheFactory.CreateMimeTypeCache, ComputeExpirationTime(cacheLifeTime));

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
            mimeType = mimeType.Replace(" ", "").ToLowerInvariant();
            ConcurrentDictionary<string, string> dic = _cache.GetOrAdd(EXTENSION_CACHE_NAME, CacheFactory.CreateFileTypeCache, ComputeExpirationTime(cacheLifeTime));

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
