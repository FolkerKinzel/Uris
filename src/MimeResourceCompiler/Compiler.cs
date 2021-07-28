using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Compiles the output.
    /// </summary>
    public sealed class Compiler : IDisposable
    {
        private const string DEFAULT_MIME_TYPE = "application/octet-stream";

        private readonly IApacheData _apacheData;
        private readonly IMimeFile _mimeFile;
        private readonly IIndexFile _indexFile;
        private readonly ICompiledFile _extensionFile;
        private readonly IResourceParser _defaultEntry;
        private readonly IResourceParser _addendum;
        private readonly ILogger _log;
        private readonly ICompressor _compressor;
        private bool _disposedValue;

        public Compiler(IApacheData apacheData,
                        IMimeFile mimeFile,
                        IIndexFile indexFile,
                        ICompiledFile extensionFile,
                        IResourceParser defaultEntry,
                        IResourceParser addendum,
                        ILogger log,
                        ICompressor compressor)
        {
            _apacheData = apacheData;
            _mimeFile = mimeFile;
            _indexFile = indexFile;
            this._extensionFile = extensionFile;
            this._defaultEntry = defaultEntry;
            _addendum = addendum;
            _log = log;
            _compressor = compressor;

            _log.Debug("Compiler initialized.");
        }

        public void CompileResources()
        {
            _log.Debug("Start Compiling.");
            List<Entry> list = CollectData();
            list = list
                .GroupBy(x => x.TopLevelMediaType, StringComparer.Ordinal)
                .SelectMany(group => group)
                .Distinct()
                .SkipWhile(x => x.MimeType.Equals(DEFAULT_MIME_TYPE, StringComparison.Ordinal))
                .ToList();

            _log.Debug("Start removing unreachable entries.");
            _compressor.RemoveUnreachableEntries(list);
            _log.Debug("Unreachable entries completely removed.");

            _log.Debug("Start writing the data files.");
            CompileMimeFile(list);
            CompileExtensionFile(list);

            _log.Debug("Data files completely written.");
        }

        private void CompileMimeFile(List<Entry> list)
        {
            _log.Debug("Start writing {0} and {1}.", _mimeFile.FileName, _indexFile.FileName);
            var comparer = new MimeTypeEqualityComparer();
            foreach (IGrouping<string, Entry> group in list.GroupBy(x => x.TopLevelMediaType, StringComparer.Ordinal))
            {
                _indexFile.WriteNewMediaType(group.Key, _mimeFile.GetCurrentStreamPosition(), group.Count());
                _mimeFile.WriteEntries(group.Distinct(comparer));
            }
            _log.Debug("{0} and {1} successfully written.", _mimeFile.FileName, _indexFile.FileName);
        }

        private void CompileExtensionFile(List<Entry> list)
        {
            _log.Debug("Start writing {0}.", _extensionFile.FileName);
            _extensionFile.WriteEntries(list.Distinct(new ExtensionEqualityComparer()));
            _log.Debug("{0}  successfully written..", _extensionFile.FileName);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #region private

        private List<Entry> CollectData()
        {
            _log.Debug("Start collecting the data.");
            var list = new List<Entry>(2048);
            CollectResourceFile(list, _defaultEntry);
            CollectApacheData(list);
            CollectResourceFile(list, _addendum);

            _log.Debug("Data completely collected.");

            return list;
        }

        private void CollectResourceFile(List<Entry> list, IResourceParser parser)
        {
            _log.Debug("Start parsing the resource {0}.", parser.FileName);

            Entry? entry;
            while ((entry = parser.GetNextLine()) is not null)
            {
                list.Add(entry);
            }

            _log.Debug("The resource {0} has been completely parsed.", parser.FileName);
        }


        private void CollectApacheData(List<Entry> list)
        {
            _log.Debug("Start parsing the Apache data.");
            IEnumerable<Entry>? line;
            while ((line = _apacheData.GetNextLine()) != null)
            {
                list.AddRange(line);
            }
            _log.Debug("Apache data completely parsed.");
            _apacheData.Dispose();

        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _addendum.Dispose();
                    _indexFile.Dispose();
                    _mimeFile.Dispose();
                }

                _disposedValue = true;

                _log.Debug("Compiler disposed.");
            }
        }

        #endregion
    }
}
