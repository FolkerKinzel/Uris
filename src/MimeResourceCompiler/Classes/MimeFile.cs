using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MimeResourceCompiler.Classes
{
    public sealed class MimeFile : IDisposable, IMimeFile
    {
        private const string mimeFileName = "Mime.csv";
        private const string newLine = "\n";
        private const char SEPARATOR = ' ';
        private readonly IStreamFactory _streamFactory;
        private StreamWriter? _writer;

        public MimeFile(IStreamFactory streamFactory) => _streamFactory = streamFactory;

        public void WriteLine(string mimeType, string extension)
        {
            Initialize();

            _writer.Write(mimeType);
            _writer.Write(SEPARATOR);
            _writer.WriteLine(extension);
            //_writer.Flush();
        }


        public long GetCurrentStreamPosition()
        {
            Initialize();

            return _writer.BaseStream.Position;
        }



        public void TruncateLastEmptyRow()
        {
            Initialize();

            _writer.Flush();

            Stream mimeFileStream = _writer.BaseStream;
            mimeFileStream.SetLength(mimeFileStream.Length - newLine.Length);
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
                Stream stream = _streamFactory.CreateWriteStream(mimeFileName);
                _writer = new StreamWriter(stream)
                {
                    NewLine = newLine
                };
            }
        }
    }
}
