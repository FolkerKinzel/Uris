using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Serilog;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Compiles the output.
    /// </summary>
    public sealed class Compiler : IDisposable
    {
        private readonly IApacheData _apacheData;
        private readonly IMimeFile _mimeFile;
        private readonly IIndexFile _indexFile;
        private readonly IDllCache _dllCache;
        private readonly IAddendum _addendum;
        private readonly ILogger _log;
        private bool _disposedValue;
        private string? _mediaType;

        public Compiler(IApacheData apacheData,
                        IMimeFile mimeFile,
                        IIndexFile indexFile,
                        IDllCache dllCache,
                        IAddendum addendum,
                        ILogger log)
        {
            _apacheData = apacheData;
            _mimeFile = mimeFile;
            _indexFile = indexFile;
            _dllCache = dllCache;
            _addendum = addendum;
            _log = log;
        }


        public string? MediaType
        {
            get => _mediaType;
            private set 
            {
                if (value is not null && !value.Equals(_mediaType, StringComparison.OrdinalIgnoreCase))
                {
                    _apacheData.TestApacheFile(value);
                }
                _mediaType = value;
            }
        }

        public int Line { get; private set; }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void CompileResources()
        {
            _log.Debug("Start Compiling.");

            _log.Debug("Start parsing the Apache data.");
            string? line;
            while ((line = _apacheData.GetNextLine()) != null)
            {
                ProcessApacheLine(line);
            }

            _log.Debug("Start adding the rest of the Addendum.");
            string? mediaTp = null;
            while (_addendum.TryGetNextLine(ref mediaTp, out AddendumRecord? row))
            {
                if (MediaType is null) // Die könnte nur sein, wenn das Apache file leer ist
                {
                    WriteIndex(mediaTp, true);
                }
                else if (!StringComparer.OrdinalIgnoreCase.Equals(MediaType, mediaTp))
                {
                    WriteIndex(mediaTp, false);
                }

                _log.Debug("Write addendum {addendum}.", row);
                AppendToMimeFile(row.MimeType, row.Extension);
            }

            _indexFile.WriteRowsCount(Line);

            _log.Debug("Addendum completely added.");
        }



        private void ProcessApacheLine(string line)
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
                string extension = parts[i];

                if (_dllCache.TryGetMimeTypeFromFileTypeExtension(extension, out string? cacheResult))
                {
                    if (StringComparer.OrdinalIgnoreCase.Equals(cacheResult, mimeType))
                    {
                        continue;
                    }
                }

                AppendToMimeFile(mimeType, extension);
                _ = _addendum.RemoveFromAddendum(mimeType, extension);
            }
        }

        private void WriteAddendum([DisallowNull] string? mediaType)
        {
            while (_addendum.TryGetNextLine(ref mediaType, out AddendumRecord? row))
            {
                _mimeFile.WriteRow(row.MimeType, row.Extension);
                _log.Debug("Write addendum {addendum}.", row);
                Line++;
            }
        }

        private void WriteIndex(string mediaType, bool firstIndex)
        {
            MediaType = mediaType;


            if (!firstIndex)
            {
                _indexFile.WriteRowsCount(Line);
            }

            _indexFile.WriteNewMediaType(MediaType, _mimeFile.GetCurrentStreamPosition());
            Line = 0;
        }


        private void AppendToMimeFile(string mimeType, string extension)
        {
            _mimeFile.WriteRow(mimeType, extension);
            ++Line;
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _indexFile.Dispose();
                    _mimeFile.Dispose();
                }

                _disposedValue = true;
            }
        }


        private static string GetMediaType(string mimeType) => mimeType.Substring(0, mimeType.IndexOf('/'));

    }
}
