using System.IO;
using System.Reflection;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Encapsulates functionality to load the resources.
    /// </summary>
    public class ResourceLoader : IResourceLoader
    {
        private const string README_PATH = "MimeResourceCompiler.Resources.Readme.txt";
        private const string ADDENDUM_PATH = "MimeResourceCompiler.Resources.Addendum.csv";

        /// <summary>
        /// Loads Readme.txt from the resources.
        /// </summary>
        /// <returns>Readme.txt as byte array.</returns>
        public byte[] LoadReadmeFile()
        {
            using Stream? stream = GetResourceStream(README_PATH);

            byte[] arr = new byte[stream.Length];
            _ = stream.Read(arr, 0, arr.Length);

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
