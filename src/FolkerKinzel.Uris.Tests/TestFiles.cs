using System.IO;


namespace FolkerKinzel.Uris.Tests
{
    internal static class TestFiles
    {
        private const string TEST_FILE_DIRECTORY_NAME = "TestFiles";
        private static readonly string _testFileDirectory;

        static TestFiles()
        {
            ProjectDirectory = Properties.Resources.ProjDir.Trim();
            _testFileDirectory = Path.Combine(ProjectDirectory, TEST_FILE_DIRECTORY_NAME);
        }


        internal static string[] GetAll() => Directory.GetFiles(_testFileDirectory);


        internal static string ProjectDirectory { get; }

        internal static string FolkerPng => Path.Combine(_testFileDirectory, "Folker.png");

    }
}
