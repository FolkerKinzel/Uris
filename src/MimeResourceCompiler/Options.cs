using System;
using CommandLine;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Encapsulates the command line options.
    /// </summary>
    public class Options
    {
        private readonly string? _outputPath;
        private readonly bool _createReadme;
        private readonly bool _createLogFile;
        private readonly bool _logToConsole;
        private readonly bool _createWrapper;

        public Options(string outputPath, bool createWrapper, bool createReadme, bool createLogFile, bool logToConsole)
        {
            _outputPath = outputPath;
            _createWrapper = createWrapper;
            _createReadme = createReadme;
            _createLogFile = createLogFile;
            _logToConsole = logToConsole;
        }

        [Option('p', "path", Required = false, HelpText = "Path to the directory, which gets the compiled output.")]
        public string OutputPath => _outputPath ?? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        [Option('w', "wrapper", Required = false, Default = true, HelpText = "False, to not wrap the output in a separate directory.")]
        public bool CreateWrapper => _createWrapper;

        [Option('r', "readme", Required = false, Default = true, HelpText = "False, to create no Readme.txt file.")]
        public bool CreateReadme => _createReadme;

        [Option('l', "logfile", Required = false, Default = false, HelpText = "True, to create a log file.")]
        public bool CreateLogFile => _createLogFile;

        [Option('c', "consolelog", Required = false, Default = false, HelpText = "True, to log to the console.")]
        public bool LogToConsole => _logToConsole;
    }
}
