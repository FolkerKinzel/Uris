using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeResourceCompiler.Classes;
using Serilog;

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

        public Factory(Options options, ILogger logger)
        {
            //throw new ArgumentNullException("name");

            _outputDirectory = new OutputDirectory(options.OutputPath, options.CreateWrapper, logger.ForContext<OutputDirectory>());
            var apacheData = new ApacheData(logger.ForContext<ApacheData>());
            var streamFactory = new StreamFactory(_outputDirectory, logger.ForContext<StreamFactory>());
            var mimeFile = new MimeFile(streamFactory, logger.ForContext<MimeFile>());
            var indexFile = new IndexFile(streamFactory);
            //var dllCache = new DllCache(logger.ForContext<DllCache>());
            var resourceLoader = new ResourceLoader(logger.ForContext<ResourceLoader>());
            var addendum = new Addendum(resourceLoader, logger.ForContext<Addendum>());
            _compiler = new Compiler(apacheData, mimeFile, indexFile, addendum, logger.ForContext<Compiler>());
            _readmeFile = new ReadmeFile(_outputDirectory, resourceLoader, logger.ForContext<ReadmeFile>());
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
