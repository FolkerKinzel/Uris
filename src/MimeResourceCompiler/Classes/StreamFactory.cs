using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler.Classes
{
    public sealed class StreamFactory : IStreamFactory
    {
        private readonly IOutputDirectory _outputDirectory;

        public StreamFactory(IOutputDirectory outputDirectory) => _outputDirectory = outputDirectory;

        public Stream CreateWriteStream(string fileName) => new FileStream(Path.Combine(_outputDirectory.FullName, fileName), FileMode.Create, FileAccess.Write, FileShare.None);
    }
}
