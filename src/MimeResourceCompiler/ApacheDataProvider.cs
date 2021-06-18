using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler
{
    public class ApacheDataProvider : IApacheDataProvider, IDisposable
    {
        private static readonly HttpClient _httpClient = new();
        private bool _initialized;
        private StringReader? _reader;
        private const string APACHE_URL = @"http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types";

        private Dictionary<string, object?> TestDic { get; } = new();



        public string? GetNextLine()
        {
            if(!_initialized)
            {
                Initialize();
            }

            string? line = _reader!.ReadLine();

            while (true)
            {
                if (line is null)
                {
                    return null;
                }

                if (line.TrimStart().StartsWith('#'))
                {
                    continue;
                }

                return line;
            }
        }

        private void Initialize()
        {
            string data = DownloadApacheList();
            _reader = new StringReader(data);

            _initialized = true;
        }

        private string DownloadApacheList() => _httpClient.GetStringAsync(APACHE_URL).Result;


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
