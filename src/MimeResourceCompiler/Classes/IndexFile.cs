using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Represents the index file MimeIdx.csv.
    /// </summary>
    public sealed class IndexFile : IDisposable, IIndexFile
    {
        private const string INDEX_FILE_NAME = "MimeIdx.csv";
        private const string NEW_LINE = "\n";
        private const char SEPARATOR = ' ';

        private readonly StreamWriter _writer;
        private readonly ILogger _log;


        public IndexFile(IStreamFactory streamFactory, ILogger log)
        {
            Stream stream = streamFactory.CreateWriteStream(INDEX_FILE_NAME);
            _writer = new StreamWriter(stream)
            {
                NewLine = NEW_LINE
            };
            this._log = log;
        }

        public string FileName => INDEX_FILE_NAME;


        /// <summary>
        /// Writes the index for the media type to the file.
        /// </summary>
        /// <param name="mediaType">First part of the Internet media type (mediatype/subtype).</param>
        /// <param name="startPosition">Byte index in Mime.csv.</param>
        /// <param name="rowsCount">The number of rows of a media type in Mime.csv.</param>
        public void WriteNewMediaType(string mediaType, long startPosition, int rowsCount)
        {
            _writer.Write(mediaType);
            _writer.Write(SEPARATOR);
            _writer.Write(startPosition);
            _writer.Write(SEPARATOR);
            _writer.WriteLine(rowsCount);
        }

        ///// <summary>
        ///// Writes the rows count of the current media type in Mime.csv to the index file.
        ///// </summary>
        ///// <param name="rowsCount">The number of rows of a media type in Mime.csv.</param>
        //public void WriteRowsCount(int linesCount) => _writer.WriteLine(linesCount);

        public void Dispose()
        {
            _writer?.Close();
            GC.SuppressFinalize(this);
            _log.Debug("{0} closed.", INDEX_FILE_NAME);
        }

    }
}
