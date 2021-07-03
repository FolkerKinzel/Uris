using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace MimeResourceCompiler.Classes
{
    /// <summary>
    /// Represents the Apache file http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types.
    /// </summary>
    public sealed class ApacheData : IApacheData, IDisposable
    {
        private const string APACHE_URL = @"http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types";

        private static readonly HttpClient _httpClient = new();
        private readonly StringReader _reader;
        private readonly Dictionary<string, object?> _testDic = new();
        private readonly ILogger _log;

        /// <summary>
        /// ctor
        /// </summary>
        public ApacheData(ILogger log)
        {
            this._log = log;

            _log.Debug("Start downloading Apache data.");
            string data = _httpClient.GetStringAsync(APACHE_URL).GetAwaiter().GetResult();
            _log.Debug("Apache data successfully loaded.");
            _reader = new StringReader(data);
        }

        /// <summary>
        /// Gets the next line with data from the apache file, or null if the file is completely read.
        /// </summary>
        /// <returns>The next line with data from the apache file or null if the file is completely read.</returns>
        public string? GetNextLine()
        {
            string? line;

            while ((line = _reader.ReadLine()) is not null)
            {
                if (line.TrimStart().StartsWith('#') || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                return line;
            }

            _log.Debug("Apache data completely parsed.");
            return null;
        }

        /// <summary>
        /// Verifies the apache file.
        /// </summary>
        /// <param name="mediaType">The first part of an Internet media type (mediatype/subtype) that's used
        /// to test the apache file.</param>
        /// <remarks>The program is based on the assertion that the apache file is ordered bei media types.</remarks>
        public void TestApacheFile(string mediaType)
        {
            try
            {
                _testDic.Add(mediaType, null);
            }
            catch (ArgumentException e)
            {
                throw new InvalidDataException($"The Apache data source {APACHE_URL} has the Media Type \"{mediaType}\" at several positions in the file.", e);
            }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void Dispose()
        {
            _reader?.Close();
            GC.SuppressFinalize(this);
        }
    }
}
