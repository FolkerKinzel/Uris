using System.IO;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Encapsulates functionality to load the resources.
    /// </summary>
    public interface IResourceLoader
    {
        /// <summary>
        /// Loads Readme.txt from the resources.
        /// </summary>
        /// <returns>Readme.txt as byte array.</returns>
        byte[] LoadReadmeFile();

        /// <summary>
        /// Returns a stream to the addendum.
        /// </summary>
        /// <returns>A stream to the addendum.</returns>
        public Stream GetAddendumStream();

    }
}