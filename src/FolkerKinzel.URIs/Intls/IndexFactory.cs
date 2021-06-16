using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.URIs.Properties;

namespace FolkerKinzel.URIs.Intls
{
    internal static class IndexFactory
    {
        private const string RESOURCE_NAME = "FolkerKinzel.URIs.Resources.MimeIdx.csv";

        internal static Dictionary<string, long> CreateIndex()
        {
            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_NAME);

            if (stream is null)
            {
                throw new InvalidDataException(string.Format(Res.ResourceNotFound, RESOURCE_NAME));
            }

            using var reader = new StreamReader(stream);

            var dic = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                const char separator = ' ';

                int separatorIndex1 = line.IndexOf(separator);
                int separatorIndex2 = line.LastIndexOf(separator);

                string mediaType = line.Substring(0, separatorIndex1);

                ++separatorIndex1;
#if NETSTANDARD2_0
                int index = int.Parse(line.Substring(separatorIndex1, separatorIndex2 - separatorIndex1));
#else
                int index = int.Parse(line.AsSpan(separatorIndex1, separatorIndex2 - separatorIndex1));
#endif
                ++separatorIndex2;

#if NETSTANDARD2_0
                int length = int.Parse(line.Substring(separatorIndex2)) - index;
#else
                int length = int.Parse(line.AsSpan(separatorIndex2)) - index;
#endif

                long l = (long)length << 32;
                l |= (long)index;

                dic[mediaType] = l;
            }

            return dic;
        }
    }
}
