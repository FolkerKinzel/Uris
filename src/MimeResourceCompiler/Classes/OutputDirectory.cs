using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Represents the output directory.
    /// </summary>
    public sealed class OutputDirectory : IOutputDirectory
    {
        private const string DIRECTORY_NAME = "Mime Resources";
        private readonly DirectoryInfo _info;
        private readonly ILogger _log;

        public OutputDirectory(string rootDirectory, bool createWrapper, ILogger log)
        {
            this._log = log;

            rootDirectory = rootDirectory is null ? Environment.CurrentDirectory : Path.GetFullPath(rootDirectory);
            
            if(createWrapper)
            {
                rootDirectory = Path.Combine(rootDirectory, DIRECTORY_NAME);
            }

            _info = Directory.CreateDirectory(rootDirectory);

            _log.Debug("Output directory {rootDirectory} successfully created.", rootDirectory);
        }

        /// <summary>
        /// Absolute path of the output directory.
        /// </summary>
        public string FullName => _info.FullName;

    }
}
