using System.IO;
using Serilog;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the readme file Readme.txt.
    /// </summary>
    public class ReadmeFile
    {
        private const string FILENAME = "Readme.txt";
        private readonly IOutputDirectory _outputDirectory;
        private readonly IResourceLoader _resourceLoader;
        private readonly ILogger _log;

        public ReadmeFile(IOutputDirectory outputDirectory, IResourceLoader resourceLoader, ILogger log)
        {
            _outputDirectory = outputDirectory;
            _resourceLoader = resourceLoader;
            this._log = log;
        }

        /// <summary>
        /// Creates the file Readme.txt.
        /// </summary>
        public void Create()
        {
            string path = Path.Combine(_outputDirectory.FullName, FILENAME);
            File.WriteAllBytes(path, _resourceLoader.LoadReadmeFile());
            _log.Debug("Readme file successfully created.");
        }
    }
}
