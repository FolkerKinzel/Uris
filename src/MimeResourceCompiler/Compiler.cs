using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MapCreations
{
    internal static class Compiler
    {
        private const string DIRECTORY_NAME = "Mime Resources";
        private const string APACHE_URL = @"http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types";
        private const char SEPARATOR = ' ';


        internal static string CreateResources(string targetDirectory)
        {
            const string defaultMime = "application/octet-stream";
            const string mimeFileName = "Mime.csv";
            const string indexFileName = "MimeIdx.csv";


            string directoryPath = CreateOutputDirectory(targetDirectory);

            string apacheList = DownloadApacheList();

            Dictionary<string, string> fileTypeCache = GetFileTypeCache();

            using var mimeFileStream = new FileStream(Path.Combine(directoryPath, mimeFileName), FileMode.Create, FileAccess.Write, FileShare.None);
            using var indexFileStream = new FileStream(Path.Combine(directoryPath, indexFileName), FileMode.Create, FileAccess.Write, FileShare.None);
            using var mimeWriter = new StreamWriter(mimeFileStream);
            using var indexWriter = new StreamWriter(indexFileStream);

            using var reader = new StringReader(apacheList);
            string? line;

            var testDic = new Dictionary<string, object?>();

            string? mediaType = null;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().StartsWith("#"))
                {
                    continue;
                }

                string[] parts = Regex.Split(line, @"\s+");

                if (parts.Length < 2 || parts[0] == defaultMime)
                {
                    continue;
                }

                string? mimeType = parts[0];

                if (mediaType is null || !mimeType.StartsWith(mediaType))
                {
                    mediaType = mimeType.Substring(0, mimeType.IndexOf('/'));

                    try
                    {
                        testDic.Add(mediaType, null);
                    }
                    catch (ArgumentException e)
                    {
                        throw new InvalidDataException($"{APACHE_URL} has the Media Type \"{mediaType}\" at different positions in the file.", e);
                    }

                    if (mimeWriter.BaseStream.Position == 0)
                    {
                        indexWriter.Write(mediaType);
                        indexWriter.Write(SEPARATOR);
                        indexWriter.Write('0');
                        indexWriter.Write(SEPARATOR);
                    }
                    else
                    {
                        long currentPosition = mimeWriter.BaseStream.Position;
                        indexWriter.WriteLine(currentPosition);

                        indexWriter.Write(mediaType);
                        indexWriter.Write(SEPARATOR);
                        indexWriter.Write(currentPosition);
                        indexWriter.Write(SEPARATOR);
                    }
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
                }
            }

            return directoryPath;
        }

        private static string DownloadApacheList()
        {
            var client = new HttpClient();
            return client.GetStringAsync(APACHE_URL).Result;
        }

        private static string CreateOutputDirectory(string targetDirectory)
        {
            string directoryPath = Path.Combine(targetDirectory, DIRECTORY_NAME);
            _ = Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }

        private static Dictionary<string, string> GetFileTypeCache() => FolkerKinzel.URIs.CacheProvider.CreateFyleTypeCache();




    }
}
