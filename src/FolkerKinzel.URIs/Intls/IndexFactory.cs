using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using FolkerKinzel.URIs.Properties;

namespace FolkerKinzel.URIs.Intls
{
    internal static class IndexFactory
    {
        private const string RESOURCE_NAME = "FolkerKinzel.URIs.Resources.MimeIdx.csv";

        internal static ConcurrentDictionary<string, long> CreateIndex()
        {
            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_NAME);

            if (stream is null)
            {
                throw new InvalidDataException(string.Format(Res.ResourceNotFound, RESOURCE_NAME));
            }

            using var reader = new StreamReader(stream);

            var dic = new ConcurrentDictionary<string, long>(StringComparer.OrdinalIgnoreCase);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                const char separator = ' ';

                int separatorIndex1 = line.IndexOf(separator);
                int separatorIndex2 = line.LastIndexOf(separator);

                string mediaType = line.Substring(0, separatorIndex1);

                ++separatorIndex1;
#if NETSTANDARD2_0
                int start = int.Parse(line.Substring(separatorIndex1, separatorIndex2 - separatorIndex1));
#else
                int start = int.Parse(line.AsSpan(separatorIndex1, separatorIndex2 - separatorIndex1));
#endif
                ++separatorIndex2;

#if NETSTANDARD2_0
                int count = int.Parse(line.Substring(separatorIndex2)) - start;
#else
                int count = int.Parse(line.AsSpan(separatorIndex2)) - start;
#endif

                dic.TryAdd(mediaType, PackIndex(start, count));
            }

            return dic;
        }


        private static long PackIndex(int start, int count)
        {
            long l = (long)count << 32;
            l |= (long)start;
            return l;
        }
    }
}
