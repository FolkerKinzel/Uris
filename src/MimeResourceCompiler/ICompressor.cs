using System.Collections.Generic;

namespace MimeResourceCompiler
{
    public interface ICompressor
    {
        /// <summary>
        /// Removes all items from <paramref name="list"/> that would never be found
        /// in a search.
        /// </summary>
        /// <param name="list">The list to work with.</param>
        void RemoveUnreachableEntries(List<Entry> list);
    }
}