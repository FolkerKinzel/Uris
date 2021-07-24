using System;
using System.Collections.Generic;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the mime file "Mime.csv".
    /// </summary>
    public interface IMimeFile : IDisposable
    {
        /// <summary>
        /// Writes the rows of data for a common media type to the MIME file.
        /// </summary>
        /// <param name="entries">The data to be written.</param>
        void WriteMediaType(IEnumerable<Entry> entries);

        /// <summary>
        /// Returns the current file position in Mime.csv.
        /// </summary>
        /// <returns>The current file position in Mime.csv.</returns>
        long GetCurrentStreamPosition();
    }
}