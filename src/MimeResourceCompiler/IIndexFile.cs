using System;

namespace MimeResourceCompiler
{
    public interface IIndexFile : IDisposable
    {
        void WriteNewMediaType(string mediaType, long startPosition);

        void WriteLinesCount(int linesCount);
    }
}