using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MimeResourceCompiler
{
    public sealed class Compiler : IDisposable
    {
        private ConcurrentDictionary<string, string> MimeTypeCache { get; } = GetMimeTypeCache();
        private readonly IApacheDataProvider _apacheProvider;
        private readonly IMimeFile _mimeFile;
        private readonly IIndexFile _indexFile;

        public Compiler(IApacheDataProvider apacheProvider, IMimeFile mimeFile, IIndexFile indexFile)
        {
            _apacheProvider = apacheProvider;
            _mimeFile = mimeFile;
            _indexFile = indexFile;
        }


        public string? MediaType { get; private set; }


        private static ConcurrentDictionary<string, string> GetMimeTypeCache() => FolkerKinzel.URIs.CacheFactory.CreateMimeTypeCache();


        public void CompileResources()
        {
            string? line;
            while ((line = _apacheProvider.GetNextLine()) != null)
            {
                ProcessLine(line);
            }//while


            // Letzte Leerzeile entfernen:
            _mimeFile.TruncateLastEmptyRow();
            
            indexWriter.Write(mimeFileStream.Length);
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

                _apacheProvider.TestApacheFile(MediaType);

                SaveIndex(mimeWriter.BaseStream.Position, indexWriter, MediaType);
            }


            for (int i = 1; i < parts.Length; i++)
            {
                string extension = $".{parts[i]}";

                if (MimeTypeCache.ContainsKey(extension))
                {
                    if (MimeTypeCache[extension] == mimeType)
                    {
                        continue;
                    }
                }

                _mimeFile.WriteLine(mimeType, extension);
            }

            ///////////////////////////////

            static void SaveIndex(long currentMimePosition, StreamWriter indexWriter, string? mediaType)
            {
                if (currentMimePosition == 0)
                {
                    indexWriter.Write(mediaType);
                    indexWriter.Write(SEPARATOR);
                    indexWriter.Write('0');
                    indexWriter.Write(SEPARATOR);
                }
                else
                {
                    indexWriter.WriteLine(currentMimePosition);

                    indexWriter.Write(mediaType);
                    indexWriter.Write(SEPARATOR);
                    indexWriter.Write(currentMimePosition);
                    indexWriter.Write(SEPARATOR);
                }
            }
        }

        public void Dispose()
        {
            this._indexFile.Dispose();
            this._mimeFile.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
