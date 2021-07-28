using System;
using System.Collections.Generic;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the mime file "Mime.csv".
    /// </summary>
    public interface IMimeFile : ICompiledFile
    {
        /// <summary>
        /// Returns the current file position in Mime.csv.
        /// </summary>
        /// <returns>The current file position in Mime.csv.</returns>
        long GetCurrentStreamPosition();
    }
}