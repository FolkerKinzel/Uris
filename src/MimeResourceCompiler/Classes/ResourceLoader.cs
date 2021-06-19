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
        public byte[] LoadReadmeFile()
        {
            using var stream = GetResourceStream("FolkerKinzel.URIs.Resources.Readme.txt");

            var arr = new byte[stream.Length];
            stream.Read(arr, 0, arr.Length);

            return arr;
        }



        private static Stream GetResourceStream(string resourceName)
        {
            Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            if (stream is null)
            {
                throw new InvalidDataException($"The resource {resourceName} has not been found.");
            }

            return stream;
        }
    }
}
