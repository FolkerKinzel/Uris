using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler
{
    internal sealed class Factory : IDisposable
    {
        private readonly IOutputDirectory _outputDirectory;
        //private readonly IApacheDataProvider _apacheProvider;
        //private readonly MimeFile _mimeFile;
        //private readonly IndexFile _indexFile;
        private readonly Compiler _compiler;

        public Factory(string rootDirectory)
        {
            _outputDirectory = new OutputDirectory(rootDirectory);
            var _apacheProvider = new ApacheDataProvider();
            var _mimeFile = new MimeFile(_outputDirectory);
            var _indexFile = new IndexFile(_outputDirectory);
            _compiler = new Compiler(_apacheProvider, _mimeFile, _indexFile);
        }

        internal IOutputDirectory ResolveOutputDirectory() => _outputDirectory;
        public Compiler ResolveCompiler() => _compiler;

        public void Dispose()
        {
            _compiler.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
