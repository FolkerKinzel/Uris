using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using FolkerKinzel.URIs.Properties;

namespace FolkerKinzel.URIs.Intls
{
    internal static class ResourceParser
    {
        private const string RESOURCE_NAME = "FolkerKinzel.URIs.Resources.Mime.csv";
        private const char SEPARATOR = ' ';
        private const string DEFAULT_MIME_TYPE = "application/octet-stream";
        private const string DEFAULT_FILE_TYPE_EXTENSION = ".bin";
        private static readonly Lazy<ConcurrentDictionary<string, long>> Index = new(IndexFactory.CreateIndex, true);


        internal static string GetMimeType(string fileTypeExtension)
        {
            using StreamReader reader = InitReader();

            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                int separatorIndex = line.IndexOf(SEPARATOR);

                ReadOnlySpan<char> span = line.AsSpan(separatorIndex + 1);

                if (span.Equals(fileTypeExtension.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return line.Substring(0, separatorIndex);
                }
            }

            return DEFAULT_MIME_TYPE;

            //////////////////////////

            static StreamReader InitReader()
            {
                Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_NAME);
                return stream is null
                    ? throw new InvalidDataException(string.Format(Res.ResourceNotFound, RESOURCE_NAME))
                    : new StreamReader(stream);
            }
        }


        internal static string GetFileType(string mimeType)
        {
            (int Start, int Count) index;

            if (Index.Value.TryGetValue(GetMediaTypeFromMimeType(mimeType), out long rawIdx))
            {
                index = UnpackIndex(rawIdx);
            }
            else
            {
                return DEFAULT_FILE_TYPE_EXTENSION;
            }

            using StringReader reader = InitReader(index);


            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                int separatorIndex = line.IndexOf(SEPARATOR);

                ReadOnlySpan<char> span = line.AsSpan(0, separatorIndex);

                if (span.Equals(mimeType.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return line.Substring(separatorIndex + 1);
                }
            }

            return DEFAULT_FILE_TYPE_EXTENSION;

            ////////////////////////////////////
            
            static StringReader InitReader((int Start, int Count) index)
            {
                using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_NAME);

                if(stream is null)
                {
                    throw new InvalidDataException(string.Format(Res.ResourceNotFound, RESOURCE_NAME));
                }

                stream.Position = index.Start;

                using var reader1 = new StreamReader(stream);
                var buf = new char[index.Count];
                _ = reader1.ReadBlock(buf, 0, buf.Length);

                return new StringReader(new string(buf));
            }

            static string GetMediaTypeFromMimeType(string mimeType)
            {
                int sepIdx = mimeType.IndexOf('/');
                return sepIdx == -1 ? mimeType : mimeType.Substring(0, sepIdx);
            }

            static (int Start, int Count) UnpackIndex(long rawIdx)
            {
                int start = (int)(rawIdx & 0xFFFFFFFF);
                int count = (int)(rawIdx >> 32);

                return (start, count);
            }

        }

        //private static StreamReader InitReader()
        //{
        //    Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_NAME);
        //    return stream is null
        //        ? throw new InvalidDataException(string.Format(Res.ResourceNotFound, RESOURCE_NAME))
        //        : new StreamReader(stream);
        //}

    }
}
