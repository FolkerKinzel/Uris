using System;
using System.Collections.Generic;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the Apache file http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types.
    /// </summary>
    public interface IApacheData : IDisposable
    {
        /// <summary>
        /// Gets the next line with data from the apache file, or null if the file is completely read.
        /// </summary>
        /// <returns>The next line with data from the apache file as a collection of <see cref="Entry"/> objects
        /// or null if the file is completely read.</returns>
        IEnumerable<Entry>? GetNextLine();
    }
}