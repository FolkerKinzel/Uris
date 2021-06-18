using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler
{
    public class OutputDirectory : IOutputDirectory
    {
        private const string DIRECTORY_NAME = "Mime Resources";
        private readonly string _rootDirectory;
        private readonly DirectoryInfo _info;

        public OutputDirectory(string rootDirectory)
        {
            this._rootDirectory = Path.GetFullPath(rootDirectory);
            this._info = Create();
        }

        public string FullName => _info.FullName;

        private DirectoryInfo Create()
        {
            string directoryPath = Path.Combine(_rootDirectory, DIRECTORY_NAME);
            return Directory.CreateDirectory(directoryPath);
        }
    }
}
