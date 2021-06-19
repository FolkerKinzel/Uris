using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MimeResourceCompiler
{
    public sealed class Compiler : IDisposable
    {
        private readonly IApacheData _apacheData;
        private readonly IMimeFile _mimeFile;
        private readonly IIndexFile _indexFile;
        private readonly IDllCache _dllCache;
        private bool _disposedValue;

        public Compiler(IApacheData apacheData, IMimeFile mimeFile, IIndexFile indexFile, IDllCache dllCache)
        {
            _apacheData = apacheData;
            _mimeFile = mimeFile;
            _indexFile = indexFile;
            _dllCache = dllCache;
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

            if (MediaType is null || !mimeType.StartsWith(MediaType, StringComparison.OrdinalIgnoreCase))
            {
                MediaType = mimeType.Substring(0, mimeType.IndexOf('/'));

                _apacheData.TestApacheFile(MediaType);

                SaveIndex();
                Line = 0;
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

                _mimeFile.WriteLine(mimeType, extension);
                ++Line;
            }
        }

        private void SaveIndex()
        {
            long currentMimeFilePosition = _mimeFile.GetCurrentStreamPosition();
            if (currentMimeFilePosition > 0)
            {
                _indexFile.WriteLinesCount(Line);
            }

            Debug.Assert(MediaType != null);
            _indexFile.WriteNewMediaType(MediaType, currentMimeFilePosition);
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

    }
}
