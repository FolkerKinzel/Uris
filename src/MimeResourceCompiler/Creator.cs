using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapCreations
{
    internal static class Creator
    {
        internal static void CreateLookup(string targetDirectory)
        {
            const string apacheUrl = @"http://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types";
            const string defaultMime = "application/octet-stream";
            const string outputFileName = "Mime.csv";
            const string separator = " .";


            using var client = new WebClient();
            string apacheList;

            apacheList = client.DownloadString(apacheUrl);

            var list = new List<string[]>(2000);

            using var stream = new FileStream(Path.Combine(targetDirectory, outputFileName), FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new StreamWriter(stream);
            using var reader = new StringReader(apacheList);
            string line;

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

                string mimeType = parts[0];

                for (int i = 1; i < parts.Length; i++)
                {
                    writer.Write(mimeType);
                    writer.Write(separator);
                    writer.WriteLine(parts[i]);
                }
            }

        }



    }
}
