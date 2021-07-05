using System;
using LazyCache;

namespace FolkerKinzel.Uris.Intls
{
    internal static class Cache
    {
        private static readonly IAppCache _cache = new CachingService();


        public static T GetOrAdd<T>(string key, Func<T> addItemFactory, TimeSpan slidingExpiration)
            => _cache.GetOrAdd<T>(key, addItemFactory, slidingExpiration);
    }
}
