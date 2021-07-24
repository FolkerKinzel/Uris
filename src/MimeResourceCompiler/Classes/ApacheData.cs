using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly StreamReader _reader;
        private readonly ILogger _log;
        private readonly List<Entry> _list = new(8);

        /// <summary>
        /// ctor
        /// </summary>
        public ApacheData(ILogger log)
        {
            this._log = log;

            _log.Debug("Start connecting to Apache data.");
            Stream data = _httpClient.GetStreamAsync(APACHE_URL).GetAwaiter().GetResult();
            _log.Debug("Apache data successfully connected.");
            _reader = new StreamReader(data);
        }

        /// <summary>
        /// Gets the next line with data from the apache file, or null if the file is completely read.
        /// </summary>
        /// <returns>The next line with data from the apache file or null if the file is completely read.</returns>
        public IEnumerable<Entry>? GetNextLine()
        {
            string? line;

            while ((line = _reader.ReadLine()) is not null)
            {
                line = line.Trim();
                if (line.StartsWith('#') || line.Length == 0)
                {
                    continue;
                }

                if (AddApacheLine(line))
                {
                    if (_list.Count != 0)
                    {
                        return _list;
                    }
                }
            }

            return null;
        }


        private bool AddApacheLine(string line)
        {
            string[] parts = Regex.Split(line, @"\s+");

            if (parts.Length < 2)
            {
                return false;
            }

            _list.Clear();

            for (int i = 1; i < parts.Length; i++)
            {
                _list.Add(new Entry(parts[0], parts[i]));
            }
            return true;
        }


        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void Dispose()
        {
            _reader?.Close();
            GC.SuppressFinalize(this);
            _log.Debug("Apache file closed.");
        }
    }
}
