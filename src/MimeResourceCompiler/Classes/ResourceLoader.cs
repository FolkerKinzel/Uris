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
        private const string README_PATH = "MimeResourceCompiler.Resources.Readme.txt";
        private const string ADDENDUM_PATH = "MimeResourceCompiler.Resources.Addendum.csv";
        private readonly ILogger _log;

        public ResourceLoader(ILogger log) => this._log = log;

        /// <summary>
        /// Loads Readme.txt from the resources.
        /// </summary>
        /// <returns>Readme.txt as byte array.</returns>
        public byte[] LoadReadmeFile()
        {
            using Stream? stream = GetResourceStream(README_PATH);

            byte[] arr = new byte[stream.Length];
            _ = stream.Read(arr, 0, arr.Length);

            _log.Debug("Readme file successfully loaded from the resources.");
            return arr;
        }

        /// <summary>
        /// Returns a stream to the addendum.
        /// </summary>
        /// <returns>A stream to the addendum.</returns>
        public Stream GetAddendumStream() => GetResourceStream(ADDENDUM_PATH);


        private static Stream GetResourceStream(string resourceName)
        {
            Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            return stream is null ? throw new InvalidDataException($"The resource {resourceName} has not been found.") : stream;
        }
    }
}
