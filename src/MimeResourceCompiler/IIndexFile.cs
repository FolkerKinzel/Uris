using System;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the index file MimeIdx.csv.
    /// </summary>
    public interface IIndexFile : IDisposable
    {
        /// <summary>
        /// Writes the new Media type and its byte index in Mime.csv to the index file.
        /// </summary>
        /// <param name="mediaType">First part of the Internet media type (mediatype/subtype).</param>
        /// <param name="startPosition">Byte index in Mime.csv.</param>
        void WriteNewMediaType(string mediaType, long startPosition);

        /// <summary>
        /// Writes the rows count of the current media type in Mime.csv to the index file.
        /// </summary>
        /// <param name="rowsCount">The number of rows of a media type in Mime.csv.</param>
        void WriteRowsCount(int rowsCount);
    }
}