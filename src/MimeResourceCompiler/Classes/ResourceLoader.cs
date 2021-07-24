using System.IO;
using System.Reflection;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Encapsulates functionality to load the resources.
    /// </summary>
    public class ResourceLoader : IResourceLoader
    {
        private const string RESOURCE_PATH = "MimeResourceCompiler.Resources.";

        /// <summary>
        /// Returns a stream to a resource.
        /// </summary>
        /// <param name="fileName">The name of the resource file.</param>
        /// <returns>A stream from the resource.</returns>
        public Stream GetResourceStream(string fileName)
        {
            Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_PATH + fileName);
            return stream is null ? throw new InvalidDataException($"The resource {fileName} has not been found.") : stream;
        }
    }
}
