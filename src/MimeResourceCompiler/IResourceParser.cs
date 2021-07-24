using System;
using System.Diagnostics.CodeAnalysis;

namespace MimeResourceCompiler
{
    public interface IResourceParser : IDisposable
    {
        /// <summary>
        /// Returns the next parsed line from the resource file.
        /// </summary>
        /// <returns>The next parsed line from Addendum.csv or <c>null</c> if EOF is reached.</returns>
        Entry? GetNextLine();

        /// <summary>
        /// The file name of the resource file.
        /// </summary>
        string FileName { get; }
        
        ///// <summary>
        ///// Removes an entry from the addendum.
        ///// </summary>
        ///// <param name="mimeType">Internet media type.</param>
        ///// <param name="extension">File type extension.</param>
        ///// <returns>true if the the entry could be removed.</returns>
        //bool RemoveFromAddendum(string mimeType, string extension);
    }
}