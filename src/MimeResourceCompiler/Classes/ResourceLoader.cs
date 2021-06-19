using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler.Classes
{
    public class ResourceLoader : IResourceLoader
    {
        const string README_PATH = "MimeResourceCompiler.Resources.Readme.txt";
        const string ADDENDUM_PATH = "MimeResourceCompiler.Resources.Addendum.csv";

        public byte[] LoadReadmeFile()
        {
            using Stream? stream = GetResourceStream(README_PATH);

            var arr = new byte[stream.Length];
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
