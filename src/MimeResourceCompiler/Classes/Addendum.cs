using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MimeResourceCompiler.Classes
{

    public class Addendum : IAddendum
    {
        private readonly SortedDictionary<string, List<AddendumRow>> _data = new(StringComparer.OrdinalIgnoreCase);

        public Addendum(IResourceLoader resourceLoader)
        {
            using var reader = new StreamReader(resourceLoader.GetAddendumStream());

            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                if (line.StartsWith('#') || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split(' ');


                if (line.StartsWith(' ') || line.EndsWith(' ') || parts.Length != 2)
                {
                    throw new InvalidDataException("The resource Addendum.csv contains invalid data.");
                }

                AddendumRow row = BuildRow(parts[0], parts[1], out string mediaType);

                if (_data.ContainsKey(mediaType))
                {
                    List<AddendumRow> list = _data[mediaType];

                    if (list.Contains(row))
                    {
                        continue;
                    }

                    list.Add(row);
                }
                else
                {
                    _data.Add(mediaType, new List<AddendumRow>() { row });
                }
            }
        }


        public bool RemoveFromAddendum(string mimeType, string extension)
        {
            AddendumRow row = BuildRow(mimeType, extension, out string mediaType);

            if (_data.ContainsKey(mediaType))
            {
                List<AddendumRow> list = _data[mediaType];

                bool result = list.Remove(row);

                if (list.Count == 0)
                {
                    _ = _data.Remove(mediaType);
                }

                return result;
            }

            return false;
        }


        public bool GetLine([NotNullWhen(true)] ref string? mediaType, [NotNullWhen(true)] out AddendumRow? row)
        {
            row = null;

            if (mediaType is null)
            {
                if (_data.Count == 0)
                {
                    return false;
                }

                KeyValuePair<string, List<AddendumRow>> kvp = _data.First();

                mediaType = kvp.Key;
                List<AddendumRow> list = kvp.Value;

                row = list[0];
                list.RemoveAt(0);

                if (list.Count == 0)
                {
                    _ = _data.Remove(mediaType);
                }

                return true;
            }
            else
            {
                if (_data.ContainsKey(mediaType))
                {
                    List<AddendumRow> list = _data[mediaType];
                    row = list[0];
                    list.RemoveAt(0);

                    if (list.Count == 0)
                    {
                        _ = _data.Remove(mediaType);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        private static AddendumRow BuildRow(string mimeType, string extension, out string mediaType)
        {
            var row = new AddendumRow(mimeType.ToLowerInvariant(), extension.ToLowerInvariant());

            int mediaTypeSeparatorIndex = row.MimeType.IndexOf('/');

            if (mediaTypeSeparatorIndex == -1)
            {
                throw new InvalidDataException();
            }

            mediaType = row.MimeType.Substring(0, mediaTypeSeparatorIndex);

            return row;
        }

    }
}
