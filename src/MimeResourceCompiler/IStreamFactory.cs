using System.IO;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Encapsulates functionality to create FileStreams to write the output.
    /// </summary>
    public interface IStreamFactory
    {
        /// <summary>
        /// Creates a FileStream to write a file named like <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">A file name without path information.</param>
        /// <returns>A FileStream to write a file named like <paramref name="fileName"/>.</returns>
        Stream CreateWriteStream(string fileName);
    }
}