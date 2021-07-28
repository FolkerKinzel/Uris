using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

#if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris.Intls
{
    /// <summary>
    /// Cache, zum Finden von Dateiendungen für MIME-Typen und für das Finden passender MIME-Typen für Dateiendungen.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    internal static class MimeCache
    {
        private class Entry //: IEquatable<Entry>
        {
            public Entry(string mimeType, string extension)
            {
                MimeType = mimeType;
                Extension = extension;
            }

            public string MimeType { get; }
            public string Extension { get; }

            public override string ToString() => $"{MimeType} {Extension}";

            //public bool Equals(Entry? other)
            // => other is not null
            //    && StringComparer.Ordinal.Equals(MimeType, other.MimeType)
            //    && StringComparer.Ordinal.Equals(Extension, other.Extension);

            //public override bool Equals(object? obj)
            // => Equals(obj as Entry);

            //public override int GetHashCode()
            //    => HashCode.Combine(MimeType, Extension);
        }

        ////////////////////////////////////////////

        private const int CACHE_MAX_SIZE = 32;
        private const int CACHE_CLEANUP_SIZE = 8;

        private static readonly object _lockObj = new ();
        private static List<Entry>? _mimeCache;
        private static List<Entry>? _extCache;


        internal static string GetMimeType(string fileTypeExtension)
        {
            fileTypeExtension = fileTypeExtension.Replace(".", null).Replace(" ", null).ToLowerInvariant();

            return TryGetMimeTypeFromCache(fileTypeExtension, out string? mimeType)
                ? mimeType
                : GetMimeTypeFromResources(fileTypeExtension);

            //////////////////////////////////////////

            static bool TryGetMimeTypeFromCache(string fileTypeExtension, [NotNullWhen(true)] out string? mimeType)
            {
                Debug.Assert(!fileTypeExtension.Contains('.'));
                Debug.Assert(!fileTypeExtension.Contains(' '));
                Debug.Assert(!fileTypeExtension.Any(c => char.IsUpper(c)));

                mimeType = default;

                lock (_lockObj)
                {
                    _extCache ??= InitExtCache();

                    if (_extCache.Find(x => x.Extension.Equals(fileTypeExtension, StringComparison.Ordinal)) is Entry entry)
                    {
                        mimeType = entry.MimeType;
                        AddEntryToExtCache(entry);

                        return true;
                    }
                }

                return false;
            }

            static string GetMimeTypeFromResources(string fileTypeExtension)
            {
                string? mimeType = ResourceParser.GetMimeType(fileTypeExtension);
                AddEntryToExtCache(new Entry(mimeType, fileTypeExtension));
                return mimeType;
            }
        }

        internal static string GetFileTypeExtension(string mimeType)
        {
            //mimeType = mimeType.Replace(" ", "").ToLowerInvariant();

            return TryGetFileTypeExtensionFromCache(mimeType, out string? fileTypeExtension)
                ? PrepareFileTypeExtension(fileTypeExtension)
                : PrepareFileTypeExtension(GetFileTypeExtensionFromResources(mimeType));

            //////////////////////////////////////////

            static bool TryGetFileTypeExtensionFromCache(string mimeType, [NotNullWhen(true)] out string? fileTypeExtension)
            {
                Debug.Assert(!mimeType.Contains(' '));
                Debug.Assert(!mimeType.Any(c => char.IsUpper(c)));

                fileTypeExtension = default;

                lock (_lockObj)
                {
                    _mimeCache ??= InitMimeCache();

                    if (_mimeCache.Find(x => x.MimeType.Equals(mimeType, StringComparison.Ordinal)) is Entry entry)
                    {
                        fileTypeExtension = entry.Extension;
                        AddEntryToMimeCache(entry);

                        return true;
                    }
                }

                return false;
            }

            static string GetFileTypeExtensionFromResources(string mimeType)
            {
                string fileTypeExtension = ResourceParser.GetFileType(mimeType);
                AddEntryToMimeCache(new Entry(mimeType, fileTypeExtension));
                return fileTypeExtension;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string PrepareFileTypeExtension(string fileTypeExtension) => $".{fileTypeExtension}";

        internal static void Clear()
        {
            lock (_lockObj)
            {
                _mimeCache = null;
                _extCache = null;
            }
        }

        ///// <summary>
        ///// Testet den vorgefüllten Cache auf Fehler.
        ///// </summary>
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //internal static void TestIt()
        //{
        //    List<Entry>? cache = InitCache();

        //    string? error = cache.Select(entry => $"{entry.MimeType} {entry.Extension}").FirstOrDefault(x => x.Any(c => char.IsUpper(c)));

        //    if(error is not null)
        //    {
        //        throw new InvalidDataException($"{nameof(FolkerKinzel.Uris.Intls)}.{nameof(MimeCache)}: The cache contains an uppercase letter at \"{error}\".");
        //    }

        //    error = cache.FirstOrDefault(entry => string.IsNullOrEmpty(entry.Extension) || entry.Extension.Contains(' ', StringComparison.Ordinal) || entry.Extension.Contains('.', StringComparison.Ordinal))?.Extension;

        //    if(error is not null)
        //    {
        //        throw new InvalidDataException($"{nameof(FolkerKinzel.Uris.Intls)}.{nameof(MimeCache)}: The cache contains an invalid file type extension at \"{error}\".");
        //    }

        //    error = cache.FirstOrDefault(entry => entry.MimeType.Contains(' ', StringComparison.Ordinal))?.MimeType;

        //    if(error is not null)
        //    {
        //        throw new InvalidDataException($"{nameof(FolkerKinzel.Uris.Intls)}.{nameof(MimeCache)}: The cache contains an invalid MIME type at \"{error}\".");
        //    }
        //}


        private static List<Entry> InitMimeCache()
            => new(CACHE_MAX_SIZE)
            {
                new ("application/json", "json" ),
                new ("application/pdf", "pdf"),
                new ("application/rtf", "rtf"),
                new ("application/xml", "xml"),
                new ("application/zip", "zip"),
                new ("image/gif", "gif"),
                new ("image/jpeg", "jpg"),
                new ("image/png", "png"),
                new ("image/svg+xml", "svg"),
                new ("message/rfc822", "eml"),
                new ("text/html", "htm"),
                new ("text/plain", "txt"),
            };


        private static List<Entry> InitExtCache()
            => new(CACHE_MAX_SIZE)
            {
                new ("application/json", "json" ),
                new ("application/pdf", "pdf"),
                new ("application/rtf", "rtf"),
                new ("application/xml", "xml"),
                new ("application/zip", "zip"),
                new ("image/gif", "gif"),
                new ("image/jpeg", "jpg"),
                new ("image/jpeg", "jpeg"),
                new ("image/png", "png"),
                new ("image/svg+xml", "svg"),
                new ("message/rfc822", "eml"),
                new ("text/html", "htm"),
                new ("text/html", "html"),
                new ("text/plain", "txt"),
                new ("text/plain", "log"),
            };

        private static void AddEntryToExtCache(Entry entry)
        {
            lock (_lockObj)
            {
                _extCache ??= InitExtCache();

                if (_extCache.Count.Equals(CACHE_MAX_SIZE))
                {
                    _extCache.RemoveRange(0, CACHE_CLEANUP_SIZE);
                }

                _ = _extCache.Remove(entry);
                _extCache.Add(entry);
            }
        }


        private static void AddEntryToMimeCache(Entry entry)
        {
            lock (_lockObj)
            {
                _mimeCache ??= InitMimeCache();

                if (_mimeCache.Count.Equals(CACHE_MAX_SIZE))
                {
                    _mimeCache.RemoveRange(0, CACHE_CLEANUP_SIZE);
                }

                _ = _mimeCache.Remove(entry);
                _mimeCache.Add(entry);
            }
        }

    }
}
