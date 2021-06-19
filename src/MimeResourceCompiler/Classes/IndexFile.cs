using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MimeResourceCompiler.Classes
{
    public sealed class IndexFile : IDisposable, IIndexFile
    {
        private const string indexFileName = "MimeIdx.csv";
        private const string newLine = "\n";
        private const char SEPARATOR = ' ';

        private StreamWriter? _writer;
        private readonly IStreamFactory _streamFactory;

        public IndexFile(IStreamFactory streamFactory) => _streamFactory = streamFactory;

        public void WriteNewMediaType(string mediaType, long startPosition)
        {
            Initialize();

            _writer.Write(mediaType);
            _writer.Write(SEPARATOR);
            _writer.Write(startPosition);
            _writer.Write(SEPARATOR);
        }

        public void WriteLinesCount(int linesCount)
        {
            Initialize();

            _writer.WriteLine(linesCount);
        }

        public void Dispose()
        {
            _writer?.Close();
            GC.SuppressFinalize(this);
        }

        [MemberNotNull(nameof(_writer))]
        private void Initialize()
        {
            if (_writer is null)
            {
                Stream stream = _streamFactory.CreateWriteStream(indexFileName);
                _writer = new StreamWriter(stream)
                {
                    NewLine = newLine
                };
            }
        }
    }
}
