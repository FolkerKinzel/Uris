using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using FolkerKinzel.Uris;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Represents the prefilled cache from the FolkerKinzel.Uris DLL.
    /// </summary>
    public sealed class DllCache : IDllCache
    {
        private readonly ConcurrentDictionary<string, string> _mimeTypeCache = CacheFactory.CreateMimeTypeCache();
        private readonly ILogger _log;

        /// <summary>
        /// ctor
        /// </summary>
        public DllCache(ILogger log)
        {
            this._log = log;

            _log.Debug("Start testing the data integrity of the cache from FolkerKinzel.Uris.");
            CacheFactory.TestIt();
            _log.Debug("Data integrity of the cache from FolkerKinzel.Uris verified.");

        }

        /// <summary>
        /// Tries to get the Internet media type for a given file type extension.
        /// </summary>
        /// <param name="extension">The file type extension.</param>
        /// <param name="mimeType">The corresponding Internet media type if the method successfully returns, otherwise null.</param>
        /// <returns>True, if the cache had an entry for <paramref name="extension"/>.</returns>
        public bool TryGetMimeTypeFromFileTypeExtension(string extension, [NotNullWhen(true)] out string? mimeType)
            => _mimeTypeCache.TryGetValue(extension.Replace(".", null, StringComparison.Ordinal).ToLowerInvariant(), out mimeType);

    }
}
