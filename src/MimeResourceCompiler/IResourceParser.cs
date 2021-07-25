using System;
using System.Diagnostics.CodeAnalysis;

namespace MimeResourceCompiler
{
    public interface IResourceParser : IDisposable
    {
        /// <summary>
        /// Returns the next parsed line from the resource file.
        /// </summary>
        /// <returns>The next parsed line from the resource file or <c>null</c> if EOF is reached.</returns>
        Entry? GetNextLine();

        /// <summary>
        /// The file name of the resource file.
        /// </summary>
        string FileName { get; }
        
    }
}