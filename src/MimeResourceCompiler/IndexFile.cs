using System;
using System.IO;

namespace MimeResourceCompiler
{
    public sealed class IndexFile : IDisposable, IIndexFile
    {
        private const string indexFileName = "MimeIdx.csv";
        private const string newLine = "\n";
        private const char SEPARATOR = ' ';


        private readonly StreamWriter _indexWriter;

        public IndexFile(IOutputDirectory outputDirectory)
        {
            var indexFileStream = new FileStream(Path.Combine(outputDirectory.FullName, indexFileName), FileMode.Create, FileAccess.Write, FileShare.None);

            _indexWriter = new StreamWriter(indexFileStream)
            {
                NewLine = newLine
            };
        }

        public void Dispose()
        {
            _indexWriter.Close();
            GC.SuppressFinalize(this);
        }
    }
}
