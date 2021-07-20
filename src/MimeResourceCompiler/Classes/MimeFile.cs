using System;
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
        /// Writes a row of data to the MIME file.
        /// </summary>
        /// <param name="mimeType">Internet media type</param>
        /// <param name="extension">File type extension.</param>
        public void WriteRow(string mimeType, string extension)
        {
            _writer.Write(mimeType.PrepareMimeType());
            _writer.Write(SEPARATOR);
            _writer.WriteLine(extension.PrepareFileTypeExtension());
            //_writer.Flush();
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
        }

        private void TruncateLastEmptyRow()
        {
            _writer.Flush();

            Stream mimeFileStream = _writer.BaseStream;
            mimeFileStream.SetLength(mimeFileStream.Length - NEW_LINE.Length);

            _log.Debug("Last empty row in {mimeFile} successfully truncated.", MIME_FILE_NAME);
        }
    }
}
