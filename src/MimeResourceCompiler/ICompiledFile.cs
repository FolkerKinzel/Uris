using System;
using System.Collections.Generic;

namespace MimeResourceCompiler
{
    public interface ICompiledFile : IDisposable
    {
        /// <summary>
        /// The filename of the compiled file. (Without path information.)
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Writes a collection of entries to the compiled file.
        /// </summary>
        /// <param name="entries">The data to be written.</param>
        void WriteEntries(IEnumerable<Entry> entries);

        ///// <summary>
        ///// Writes an <see cref="Entry"/> to the compiled file.
        ///// </summary>
        ///// <param name="entries">The data to be written.</param>
        //void WriteEntry(Entry entry);
    }
}