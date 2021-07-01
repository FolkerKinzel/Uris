using System;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the mime file "Mime.csv".
    /// </summary>
    public interface IMimeFile : IDisposable
    {
        /// <summary>
        /// Writes a row of data to the MIME file.
        /// </summary>
        /// <param name="mimeType">Internet media type</param>
        /// <param name="extension">File type extension.</param>
        void WriteRow(string mimeType, string extension);

        /// <summary>
        /// Returns the current file position in Mime.csv.
        /// </summary>
        /// <returns>The current file position in Mime.csv.</returns>
        long GetCurrentStreamPosition();
    }
}