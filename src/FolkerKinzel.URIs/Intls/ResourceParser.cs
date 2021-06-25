using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using FolkerKinzel.Uris.Properties;

namespace FolkerKinzel.Uris.Intls
{
    internal static class ResourceParser
    {
        private const string RESOURCE_NAME = "FolkerKinzel.Uris.Resources.Mime.csv";
        private const char SEPARATOR = ' ';
        private const string DEFAULT_MIME_TYPE = "application/octet-stream";
        private const string DEFAULT_FILE_TYPE_EXTENSION = ".bin";
        private static readonly Lazy<ConcurrentDictionary<string, long>> _index = new(IndexFactory.CreateIndex, true);


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
            (int Start, int LinesCount) index;

            if (_index.Value.TryGetValue(GetMediaTypeFromMimeType(mimeType), out long rawIdx))
            {
                index = UnpackIndex(rawIdx);
            }
            else
            {
                return DEFAULT_FILE_TYPE_EXTENSION;
            }

            using StreamReader reader = InitReader(index.Start);

            for (int i = 0; i < index.LinesCount; i++)
            {
                string? line = reader.ReadLine();

                if(line is null)
                {
                    break;
                }
            
                int separatorIndex = line.IndexOf(SEPARATOR);

                ReadOnlySpan<char> span = line.AsSpan(0, separatorIndex);

                if (span.Equals(mimeType.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return line.Substring(separatorIndex + 1);
                }
            }

            return DEFAULT_FILE_TYPE_EXTENSION;

            ////////////////////////////////////
            
            static StreamReader InitReader(int start)
            {
                Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(RESOURCE_NAME);

                if(stream is null)
                {
                    throw new InvalidDataException(string.Format(Res.ResourceNotFound, RESOURCE_NAME));
                }

                stream.Position = start;

                return new StreamReader(stream);
            }

            static string GetMediaTypeFromMimeType(string mimeType)
            {
                int sepIdx = mimeType.IndexOf('/');
                return sepIdx == -1 ? mimeType : mimeType.Substring(0, sepIdx);
            }

            static (int Start, int LinesCount) UnpackIndex(long rawIdx)
            {
                int start = (int)(rawIdx & 0xFFFFFFFF);
                int linesCount = (int)(rawIdx >> 32);

                return (start, linesCount);
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
