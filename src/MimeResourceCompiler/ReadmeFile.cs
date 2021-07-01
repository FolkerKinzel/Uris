using System.IO;

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

        public ReadmeFile(IOutputDirectory outputDirectory, IResourceLoader resourceLoader)
        {
            _outputDirectory = outputDirectory;
            _resourceLoader = resourceLoader;
        }

        /// <summary>
        /// Creates the file Readme.txt.
        /// </summary>
        public void Create()
        {
            string path = Path.Combine(_outputDirectory.FullName, FILENAME);
            File.WriteAllBytes(path, _resourceLoader.LoadReadmeFile());
        }
    }
}
