using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MimeResourceCompiler
{
    public sealed class Compiler : IDisposable
    {
        private readonly IApacheData _apacheData;
        private readonly IMimeFile _mimeFile;
        private readonly IIndexFile _indexFile;
        private readonly IDllCache _dllCache;
        private readonly IAddendum _addendum;
        private bool _disposedValue;

        public Compiler(IApacheData apacheData,
                        IMimeFile mimeFile,
                        IIndexFile indexFile,
                        IDllCache dllCache,
                        IAddendum addendum)
        {
            _apacheData = apacheData;
            _mimeFile = mimeFile;
            _indexFile = indexFile;
            _dllCache = dllCache;
            _addendum = addendum;
        }


        public string? MediaType { get; private set; }

        public int Line { get; private set; }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void CompileResources()
        {
            string? line;
            while ((line = _apacheData.GetNextLine()) != null)
            {
                ProcessLine(line);
            }


            string? mediaTp = null;
            while (_addendum.GetLine(ref mediaTp, out AddendumRow? row))
            {
                if (MediaType is null) // Die könnte nur sein, wenn das Apache file leer ist
                {
                    WriteIndex(mediaTp, true);
                }
                else if (!StringComparer.OrdinalIgnoreCase.Equals(MediaType, mediaTp))
                {
                    WriteIndex(mediaTp, false);
                }

                WriteMimeFile(row.MimeType, row.Extension);
            }

            _indexFile.WriteLinesCount(Line);


            // Letzte Leerzeile entfernen:
            _mimeFile.TruncateLastEmptyRow();
        }

        

        private void ProcessLine(string line)
        {
            const string defaultMime = "application/octet-stream";

            string[] parts = Regex.Split(line, @"\s+");

            if (parts.Length < 2 || parts[0] == defaultMime)
            {
                return;
            }

            string? mimeType = parts[0];

            if (MediaType is null)
            {
                WriteIndex(GetMediaType(mimeType), true);
            }
            else if (!mimeType.StartsWith(MediaType, StringComparison.OrdinalIgnoreCase))
            {
                WriteAddendum(MediaType);
                WriteIndex(GetMediaType(mimeType), false);
            }


            for (int i = 1; i < parts.Length; i++)
            {
                string extension = $".{parts[i]}";

                if (_dllCache.TryGetMimeTypeFromFileTypeExtension(extension, out string? cacheResult))
                {
                    if (cacheResult == mimeType)
                    {
                        continue;
                    }
                }

                WriteMimeFile(mimeType, extension);
                _ = _addendum.RemoveFromAddendum(mimeType, extension);
            }
        }

        private void WriteAddendum([DisallowNull] string? mediaType)
        {
            while (_addendum.GetLine(ref mediaType, out AddendumRow? row))
            {
                _mimeFile.WriteLine(row.MimeType, row.Extension);
                Line++;
            }
        }

        private void WriteIndex(string mediaType, bool firstIndex)
        {
            MediaType = mediaType;
            _apacheData.TestApacheFile(MediaType);

            if (!firstIndex)
            {
                _indexFile.WriteLinesCount(Line);
            }

            _indexFile.WriteNewMediaType(MediaType, _mimeFile.GetCurrentStreamPosition());
            Line = 0;
        }


        private void WriteMimeFile(string mimeType, string extension)
        {
            _mimeFile.WriteLine(mimeType, extension);
            ++Line;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    this._indexFile.Dispose();
                    this._mimeFile.Dispose();
                }

                _disposedValue = true;
            }
        }


        private static string GetMediaType(string mimeType) => mimeType.Substring(0, mimeType.IndexOf('/'));

    }
}
