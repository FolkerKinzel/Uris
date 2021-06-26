using System.IO;
using System.Reflection;

namespace MimeResourceCompiler.Classes
{
    public class ResourceLoader : IResourceLoader
    {
        private const string README_PATH = "MimeResourceCompiler.Resources.Readme.txt";
        private const string ADDENDUM_PATH = "MimeResourceCompiler.Resources.Addendum.csv";

        public byte[] LoadReadmeFile()
        {
            using Stream? stream = GetResourceStream(README_PATH);

            byte[] arr = new byte[stream.Length];
            _ = stream.Read(arr, 0, arr.Length);

            return arr;
        }


        public Stream GetAddendumStream() => GetResourceStream(ADDENDUM_PATH);


        private static Stream GetResourceStream(string resourceName)
        {
            Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            return stream is null ? throw new InvalidDataException($"The resource {resourceName} has not been found.") : stream;
        }
    }
}
