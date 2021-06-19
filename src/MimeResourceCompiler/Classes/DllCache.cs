using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler.Classes
{
    public sealed class DllCache : IDllCache
    {
        private readonly ConcurrentDictionary<string, string> _mimeTypeCache = FolkerKinzel.URIs.CacheFactory.CreateMimeTypeCache();

        public bool TryGetMimeTypeFromFileTypeExtension(string extension, [NotNullWhen(true)] out string? mimeType)
            => _mimeTypeCache.TryGetValue(extension, out mimeType);

    }
}
