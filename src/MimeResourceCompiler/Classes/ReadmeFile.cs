using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler.Classes
{
    public class ReadmeFile
    {
        const string FILENAME = "Readme.txt";
        private readonly IOutputDirectory _outputDirectory;
        private readonly IResourceLoader _resourceLoader;

        public ReadmeFile(IOutputDirectory outputDirectory, IResourceLoader resourceLoader)
        {
            this._outputDirectory = outputDirectory;
            this._resourceLoader = resourceLoader;
        }


        public void Create()
        {
            string path = Path.Combine(_outputDirectory.FullName, FILENAME);
            File.WriteAllBytes(path, _resourceLoader.LoadReadmeFile());
        }
    }
}
