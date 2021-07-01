using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeResourceCompiler.Classes;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Builds the objects used in the program and takes care to release their resources.
    /// </summary>
    internal sealed class Factory : IDisposable
    {
        private readonly IOutputDirectory _outputDirectory;
        private readonly Compiler _compiler;
        private readonly ReadmeFile _readmeFile;

        public Factory(Options options)
        {
            _outputDirectory = new OutputDirectory(options.OutputPath, options.CreateWrapper);
            var apacheProvider = new ApacheData();
            var streamFactory = new StreamFactory(_outputDirectory);
            var mimeFile = new MimeFile(streamFactory);
            var indexFile = new IndexFile(streamFactory);
            var dllCache = new DllCache();
            var resourceLoader = new ResourceLoader();
            var addendum = new Addendum(resourceLoader);
            _compiler = new Compiler(apacheProvider, mimeFile, indexFile, dllCache, addendum);
            _readmeFile = new ReadmeFile(_outputDirectory, resourceLoader);
        }

        public IOutputDirectory ResolveOutputDirectory() => _outputDirectory;

        public Compiler ResolveCompiler() => _compiler;

        public ReadmeFile ResolveReadmeFile() => _readmeFile;

        public void Dispose()
        {
            _compiler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
