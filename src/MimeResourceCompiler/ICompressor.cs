using System.Collections.Generic;

namespace MimeResourceCompiler
{
    public interface ICompressor
    {
        void RemoveUnreachableEntries(List<Entry> list);
    }
}