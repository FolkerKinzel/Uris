using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Encapsulates functionality to create FileStreams to write the output.
    /// </summary>
    public sealed class StreamFactory : IStreamFactory
    {
        private readonly IOutputDirectory _outputDirectory;

        public StreamFactory(IOutputDirectory outputDirectory) => _outputDirectory = outputDirectory;

        /// <summary>
        /// Creates a FileStream to write a file named like <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">A file name without path information.</param>
        /// <returns>A FileStream to write a file named like <paramref name="fileName"/>.</returns>
        public Stream CreateWriteStream(string fileName) => new FileStream(Path.Combine(_outputDirectory.FullName, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
    }
}
