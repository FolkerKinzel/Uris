using System.IO;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Encapsulates functionality to load the resources.
    /// </summary>
    public interface IResourceLoader
    {
        /// <summary>
        /// Returns a stream to a resource.
        /// </summary>
        /// <param name="fileName">The name of the resource file.</param>
        /// <returns>A stream to the resource.</returns>
        Stream GetResourceStream(string fileName);

    }
}