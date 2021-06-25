using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using FolkerKinzel.Uris;

namespace MimeResourceCompiler.Classes
{
    public sealed class DllCache : IDllCache
    {
        private readonly ConcurrentDictionary<string, string> _mimeTypeCache = CacheFactory.CreateMimeTypeCache();

        public DllCache() => CacheFactory.TestIt();

        public bool TryGetMimeTypeFromFileTypeExtension(string extension, [NotNullWhen(true)] out string? mimeType)
            => _mimeTypeCache.TryGetValue(extension, out mimeType);

    }
}
