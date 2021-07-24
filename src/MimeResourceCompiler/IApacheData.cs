using System.Collections.Generic;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the Apache file http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types.
    /// </summary>
    public interface IApacheData
    {
        /// <summary>
        /// Gets the next line with data from the apache file, or null if the file is completely read.
        /// </summary>
        /// <returns>The next line with data from the apache file or null if the file is completely read.</returns>
        IEnumerable<Entry>? GetNextLine();

        ///// <summary>
        ///// Verifies the apache file.
        ///// </summary>
        ///// <param name="mediaType">The first part of an Internet media type (mediatype/subtype) that's used
        ///// to test the apache file.</param>
        ///// <remarks>The program is based on the assertion that the apache file is ordered bei media types.</remarks>
        //void TestApacheFile(string mediaType);
    }
}