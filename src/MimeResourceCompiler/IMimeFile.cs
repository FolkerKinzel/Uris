using System;

namespace MimeResourceCompiler
{
    public interface IMimeFile : IDisposable
    {
        void TruncateLastEmptyRow();

        void WriteLine(string mimeType, string extension)
    }
}