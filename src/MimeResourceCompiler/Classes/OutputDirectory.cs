using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler.Classes
{
    public sealed class OutputDirectory : IOutputDirectory
    {
        private const string DIRECTORY_NAME = "Mime Resources";
        private readonly DirectoryInfo _info;

        public OutputDirectory(string rootDirectory, bool createWrapper)
        {
            rootDirectory = rootDirectory is null ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) : Path.GetFullPath(rootDirectory);
            
            if(createWrapper)
            {
                rootDirectory = Path.Combine(rootDirectory, DIRECTORY_NAME);
            }

            this._info = Directory.CreateDirectory(rootDirectory);
        }

        public string FullName => _info.FullName;

    }
}
