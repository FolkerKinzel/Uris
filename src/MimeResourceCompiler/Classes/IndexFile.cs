using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MimeResourceCompiler.Classes
{
    public sealed class IndexFile : IDisposable, IIndexFile
    {
        private const string INDEX_FILE_NAME = "MimeIdx.csv";
        private const string NEW_LINE = "\n";
        private const char SEPARATOR = ' ';

        private readonly StreamWriter _writer;

        public IndexFile(IStreamFactory streamFactory)
        {
            Stream stream = streamFactory.CreateWriteStream(INDEX_FILE_NAME);
            _writer = new StreamWriter(stream)
            {
                NewLine = NEW_LINE
            };
        }

        public void WriteNewMediaType(string mediaType, long startPosition)
        {
            _writer.Write(mediaType);
            _writer.Write(SEPARATOR);
            _writer.Write(startPosition);
            _writer.Write(SEPARATOR);
        }

        public void WriteLinesCount(int linesCount) => _writer.WriteLine(linesCount);

        public void Dispose()
        {
            _writer?.Close();
            GC.SuppressFinalize(this);
        }

    }
}
