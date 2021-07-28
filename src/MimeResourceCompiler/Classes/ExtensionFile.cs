using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Represents the compiled file "Extension.csv".
    /// </summary>
    public sealed class ExtensionFile : CompiledFile
    {
        public ExtensionFile(IStreamFactory streamFactory, ILogger log) : base(streamFactory, log) { }

        /// <summary>
        /// The filename of the compiled file. (Without path information.)
        /// </summary>
        public override string FileName => "Extension.csv";
        
    }
}
