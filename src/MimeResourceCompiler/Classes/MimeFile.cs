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
        private readonly StreamWriter _writer;

        public MimeFile(IStreamFactory streamFactory)
        {
            Stream stream = streamFactory.CreateWriteStream(mimeFileName);
            _writer = new StreamWriter(stream)
            {
                NewLine = newLine
            };
        }

        public void WriteLine(string mimeType, string extension)
        {
            _writer.Write(mimeType);
            _writer.Write(SEPARATOR);
            _writer.WriteLine(extension);
            //_writer.Flush();
        }


        public long GetCurrentStreamPosition()
        {
            _writer.Flush();
            return _writer.BaseStream.Position;
        }

        public void TruncateLastEmptyRow()
        {
            _writer.Flush();

            Stream mimeFileStream = _writer.BaseStream;
            mimeFileStream.SetLength(mimeFileStream.Length - newLine.Length);
        }

        public void Dispose()
        {
            _writer?.Close();
            GC.SuppressFinalize(this);
        }

        //[MemberNotNull(nameof(_writer))]
        //private void Initialize()
        //{
        //    if (_writer is null)
        //    {
        //        Stream stream = _streamFactory.CreateWriteStream(mimeFileName);
        //        _writer = new StreamWriter(stream)
        //        {
        //            NewLine = newLine
        //        };
        //    }
        //}
    }
}
