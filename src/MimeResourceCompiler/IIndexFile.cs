using System;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the index file MimeIdx.csv.
    /// </summary>
    public interface IIndexFile : IDisposable
    {
        /// <summary>
        /// Writes the index for the media type to the file.
        /// </summary>
        /// <param name="mediaType">First part of the Internet media type (mediatype/subtype).</param>
        /// <param name="startPosition">Byte index in Mime.csv where the <paramref name="mediaType"/> starts.</param>
        /// <param name="rowsCount">The number of rows of <paramref name="mediaType"/> in Mime.csv.</param>
        void WriteNewMediaType(string mediaType, long startPosition, int rowsCount);

        string FileName { get; }
    }
}