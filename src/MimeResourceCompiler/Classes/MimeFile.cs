using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Represents the mime file "Mime.csv".
    /// </summary>
    public sealed class MimeFile : IDisposable, IMimeFile
    {
        private const string MIME_FILE_NAME = "Mime.csv";
        private const string NEW_LINE = "\n";
        private const char SEPARATOR = ' ';
        private readonly StreamWriter _writer;
        private readonly ILogger _log;

        public MimeFile(IStreamFactory streamFactory, ILogger log)
        {
            this._log = log;

            Stream stream = streamFactory.CreateWriteStream(MIME_FILE_NAME);
            _writer = new StreamWriter(stream)
            {
                NewLine = NEW_LINE
            };
        }

        /// <summary>
        /// Writes the rows of data for a common media type to the MIME file.
        /// </summary>
        /// <param name="entries">The data to be written.</param>
        public void WriteMediaType(IEnumerable<Entry> entries)
        {
            foreach (Entry entry in entries)
            {
                _writer.Write(entry.MimeType);
                _writer.Write(SEPARATOR);
                _writer.WriteLine(entry.Extension);
            }
        }

        /// <summary>
        /// Returns the current file position in Mime.csv.
        /// </summary>
        /// <returns>The current file position in Mime.csv.</returns>
        public long GetCurrentStreamPosition()
        {
            _writer.Flush();
            return _writer.BaseStream.Position;
        }

        public void Dispose()
        {
            TruncateLastEmptyRow();
            _writer?.Close();
            GC.SuppressFinalize(this);
            _log.Debug("{0} closed.", MIME_FILE_NAME);
        }

        private void TruncateLastEmptyRow()
        {
            _writer.Flush();

            Stream mimeFileStream = _writer.BaseStream;
            long newLength = mimeFileStream.Length - NEW_LINE.Length;
            mimeFileStream.SetLength(newLength > 0 ? newLength : 0);

            _log.Debug("Last empty row in {mimeFile} successfully truncated.", MIME_FILE_NAME);
        }
    }
}
