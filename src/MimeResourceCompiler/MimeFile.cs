using System;
using System.IO;

namespace MimeResourceCompiler
{
    public sealed class MimeFile : IDisposable, IMimeFile
    {
        private const string mimeFileName = "Mime.csv";
        private const string newLine = "\n";
        private const char SEPARATOR = ' ';
        private readonly IOutputDirectory _outputDirectory;
        private StreamWriter? _mimeWriter;
        private bool _initialized;

        public MimeFile(IOutputDirectory outputDirectory)
        {
            _outputDirectory = outputDirectory;

        }

        public void WriteLine(string mimeType, string extension)
        {
            if (!_initialized)
            {
                Initialize();
            }

            _mimeWriter!.Write(mimeType);
            _mimeWriter.Write(SEPARATOR);
            _mimeWriter.WriteLine(extension);
            _mimeWriter.Flush();
        }

        private void Initialize()
        {
            FileStream mimeFileStream = new(Path.Combine(_outputDirectory.FullName, mimeFileName), FileMode.Create, FileAccess.Write, FileShare.None);
            _mimeWriter = new StreamWriter(mimeFileStream)
            {
                NewLine = newLine
            };

            _initialized = true;
        }

        public void TruncateLastEmptyRow()
        {
            if (!_initialized)
            {
                Initialize();
            }

            _mimeWriter!.Flush();

            Stream mimeFileStream = _mimeWriter.BaseStream;
            mimeFileStream.SetLength(mimeFileStream.Length - newLine.Length);
        }

        public void Dispose()
        {
            _mimeWriter?.Close();
            GC.SuppressFinalize(this);
        }
    }
}
