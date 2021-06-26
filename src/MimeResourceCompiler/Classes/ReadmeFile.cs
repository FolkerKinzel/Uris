using System.IO;

namespace MimeResourceCompiler.Classes
{
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


        public void Create()
        {
            string path = Path.Combine(_outputDirectory.FullName, FILENAME);
            File.WriteAllBytes(path, _resourceLoader.LoadReadmeFile());
        }
    }
}
