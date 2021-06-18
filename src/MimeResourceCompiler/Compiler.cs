using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MapCreations
{
    internal class Compiler
    {
        private static readonly HttpClient _httpClient = new();

        private const string DIRECTORY_NAME = "Mime Resources";
        private const string APACHE_URL = @"http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types";
        private const char SEPARATOR = ' ';

        private string? MediaType { get; set; }
        internal string OutputDirectory { get; }

        private ConcurrentDictionary<string, string> MimeTypeCache { get; } = GetMimeTypeCache();
        private Dictionary<string, object?> TestDic { get; } = new();


        public Compiler(string targetDirectory) => OutputDirectory = CreateOutputDirectory(targetDirectory);

        internal void CreateResources() => CompileResources(DownloadApacheList());

        private static string CreateOutputDirectory(string targetDirectory)
        {
            string directoryPath = Path.Combine(targetDirectory, DIRECTORY_NAME);
            _ = Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }

        private static string DownloadApacheList() => _httpClient.GetStringAsync(APACHE_URL).Result;

        private static ConcurrentDictionary<string, string> GetMimeTypeCache() => FolkerKinzel.URIs.CacheFactory.CreateMimeTypeCache();


        private void CompileResources(string apacheList)
        {
            const string mimeFileName = "Mime.csv";
            const string indexFileName = "MimeIdx.csv";
            const string newLine = "\n";

            using var mimeFileStream = new FileStream(Path.Combine(OutputDirectory, mimeFileName), FileMode.Create, FileAccess.Write, FileShare.None);
            using var indexFileStream = new FileStream(Path.Combine(OutputDirectory, indexFileName), FileMode.Create, FileAccess.Write, FileShare.None);
            using var mimeWriter = new StreamWriter(mimeFileStream);
            mimeWriter.NewLine = newLine;
            using var indexWriter = new StreamWriter(indexFileStream);
            indexWriter.NewLine = newLine;
            using var reader = new StringReader(apacheList);
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                ProcessLine(mimeWriter, indexWriter, line);
            }//while


            // Letzte Leerzeile entfernen:
            mimeWriter.Flush();
            mimeFileStream.SetLength(mimeFileStream.Length - newLine.Length);


            indexWriter.Write(mimeFileStream.Length);
        }

        private void ProcessLine(StreamWriter mimeWriter,
                                 StreamWriter indexWriter,
                                 string line)
        {
            const string defaultMime = "application/octet-stream";

            if (line.Trim().StartsWith('#'))
            {
                return;
            }

            string[] parts = Regex.Split(line, @"\s+");

            if (parts.Length < 2 || parts[0] == defaultMime)
            {
                return;
            }

            string? mimeType = parts[0];

            if (MediaType is null || !mimeType.StartsWith(MediaType, StringComparison.OrdinalIgnoreCase))
            {
                MediaType = mimeType.Substring(0, mimeType.IndexOf('/'));

                TestApacheFile(MediaType);

                SaveIndex(mimeWriter.BaseStream.Position, indexWriter, MediaType);
            }


            for (int i = 1; i < parts.Length; i++)
            {
                string extension = $".{parts[i]}";

                if (MimeTypeCache.ContainsKey(extension))
                {
                    if (MimeTypeCache[extension] == mimeType)
                    {
                        continue;
                    }
                }

                mimeWriter.Write(mimeType);
                mimeWriter.Write(SEPARATOR);
                mimeWriter.WriteLine(extension);
                mimeWriter.Flush();
            }

            ///////////////////////////////

            static void SaveIndex(long currentMimePosition, StreamWriter indexWriter, string? mediaType)
            {
                if (currentMimePosition == 0)
                {
                    indexWriter.Write(mediaType);
                    indexWriter.Write(SEPARATOR);
                    indexWriter.Write('0');
                    indexWriter.Write(SEPARATOR);
                }
                else
                {
                    indexWriter.WriteLine(currentMimePosition);

                    indexWriter.Write(mediaType);
                    indexWriter.Write(SEPARATOR);
                    indexWriter.Write(currentMimePosition);
                    indexWriter.Write(SEPARATOR);
                }
            }
        }

        private void TestApacheFile(string mediaType)
        {
            try
            {
                TestDic.Add(mediaType, null);
            }
            catch (ArgumentException e)
            {
                throw new InvalidDataException($"{APACHE_URL} has the Media Type \"{mediaType}\" at different positions in the file.", e);
            }
        }

    }
}
