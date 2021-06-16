using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MapCreations
{
    internal static class Compiler
    {
        private static readonly HttpClient _httpClient = new();

        private const string DIRECTORY_NAME = "Mime Resources";
        private const string APACHE_URL = @"http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types";
        private const char SEPARATOR = ' ';


        internal static string CreateResources(string targetDirectory)
        {
            string directoryPath = CreateOutputDirectory(targetDirectory);

            CompileResources(directoryPath, DownloadApacheList(), GetFileTypeCache());

            return directoryPath;
        }

        private static string CreateOutputDirectory(string targetDirectory)
        {
            string directoryPath = Path.Combine(targetDirectory, DIRECTORY_NAME);
            _ = Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }

        private static string DownloadApacheList() => _httpClient.GetStringAsync(APACHE_URL).Result;

        private static Dictionary<string, string> GetFileTypeCache() => FolkerKinzel.URIs.CacheFactory.CreateFyleTypeCache();


        private static void CompileResources(string directoryPath, string apacheList, Dictionary<string, string> fileTypeCache)
        {
            const string mimeFileName = "Mime.csv";
            const string indexFileName = "MimeIdx.csv";
            const string newLine = "\n";

            using var mimeFileStream = new FileStream(Path.Combine(directoryPath, mimeFileName), FileMode.Create, FileAccess.Write, FileShare.None);
            using var indexFileStream = new FileStream(Path.Combine(directoryPath, indexFileName), FileMode.Create, FileAccess.Write, FileShare.None);
            using var mimeWriter = new StreamWriter(mimeFileStream);
            mimeWriter.NewLine = newLine;
            using var indexWriter = new StreamWriter(indexFileStream);
            indexWriter.NewLine = newLine;
            using var reader = new StringReader(apacheList);
            string? line;

            var testDic = new Dictionary<string, object?>();

            string? mediaType = null;

            while ((line = reader.ReadLine()) != null)
            {
                ProcessLine(fileTypeCache, mimeWriter, indexWriter, line, testDic, ref mediaType);
            }//while

            
            mimeWriter.Flush();
            mimeFileStream.SetLength(mimeFileStream.Length - newLine.Length);
            indexWriter.Write(mimeFileStream.Length);
        }

        private static void ProcessLine(Dictionary<string, string> fileTypeCache, StreamWriter mimeWriter, StreamWriter indexWriter, string line, Dictionary<string, object?> testDic, ref string? mediaType)
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

            if (mediaType is null || !mimeType.StartsWith(mediaType))
            {
                mediaType = mimeType.Substring(0, mimeType.IndexOf('/'));

                TestApacheFile(testDic, mediaType);

                SaveIndex(mimeWriter.BaseStream.Position, indexWriter, mediaType);
            }


            for (int i = 1; i < parts.Length; i++)
            {
                string extension = $".{parts[i]}";

                if (fileTypeCache.ContainsKey(extension))
                {
                    if (fileTypeCache[extension] == mimeType)
                    {
                        continue;
                    }
                }

                mimeWriter.Write(mimeType);
                mimeWriter.Write(SEPARATOR);
                mimeWriter.WriteLine(extension);
                mimeWriter.Flush();
            }

            ///////////////////////////////////

            static void TestApacheFile(Dictionary<string, object?> testDic, string mediaType)
            {
                try
                {
                    testDic.Add(mediaType, null);
                }
                catch (ArgumentException e)
                {
                    throw new InvalidDataException($"{APACHE_URL} has the Media Type \"{mediaType}\" at different positions in the file.", e);
                }
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



    }
}
