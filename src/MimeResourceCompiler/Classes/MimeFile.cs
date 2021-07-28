using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Represents the compiled file "Mime.csv".
    /// </summary>
    public sealed class MimeFile : CompiledFile, IMimeFile
    {
        public MimeFile(IStreamFactory streamFactory, ILogger log) : base(streamFactory, log) { }

        /// <summary>
        /// The filename of the compiled file. (Without path information.)
        /// </summary>
        public override string FileName => "Mime.csv";

        /// <summary>
        /// Returns the current file position in Mime.csv.
        /// </summary>
        /// <returns>The current file position in Mime.csv.</returns>
        public long GetCurrentStreamPosition()
        {
            _writer.Flush();
            return _writer.BaseStream.Position;
        }
        
        
    }
}
