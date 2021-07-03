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
    /// Encapsulates functionality to create FileStreams to write the output.
    /// </summary>
    public sealed class StreamFactory : IStreamFactory
    {
        private readonly IOutputDirectory _outputDirectory;
        private readonly ILogger _log;

        public StreamFactory(IOutputDirectory outputDirectory, ILogger log)
        {
            _outputDirectory = outputDirectory;
            this._log = log;
        }

        /// <summary>
        /// Creates a FileStream to write a file named like <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">A file name without path information.</param>
        /// <returns>A FileStream to write a file named like <paramref name="fileName"/>.</returns>
        public Stream CreateWriteStream(string fileName)
        {
            var fs = new FileStream(Path.Combine(_outputDirectory.FullName, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
            _log.Debug("Created FileStream for {fileName}.", fileName);
            return fs;
        }
    }
}
