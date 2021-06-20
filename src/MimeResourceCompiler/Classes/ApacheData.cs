using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler.Classes
{
    public sealed class ApacheData : IApacheData, IDisposable
    {
        private static readonly HttpClient _httpClient = new();
        private readonly StringReader _reader;
        private const string APACHE_URL = @"http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types";

        private Dictionary<string, object?> TestDic { get; } = new();

        public ApacheData()
        {
            string data = _httpClient.GetStringAsync(APACHE_URL).GetAwaiter().GetResult();
            _reader = new StringReader(data);
        }

        public string? GetNextLine()
        {
            string? line;

            while ((line = _reader.ReadLine()) is not null)
            {
                if (line.TrimStart().StartsWith('#'))
                {
                    continue;
                }

                return line;
            }

            return null;
        }

        public void TestApacheFile(string mediaType)
        {
            try
            {
                TestDic.Add(mediaType, null);
            }
            catch (ArgumentException e)
            {
                throw new InvalidDataException($"The Apache data source {APACHE_URL} has the Media Type \"{mediaType}\" at several positions in the file.", e);
            }
        }

        public void Dispose()
        {
            _reader?.Close();
            GC.SuppressFinalize(this);
        }
    }
}
