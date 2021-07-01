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

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="resourceLoader">IResourceLoader</param>
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

                string[] parts = line.Split(' ');


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


        /// <summary>
        /// Removes an entry from the addendum.
        /// </summary>
        /// <param name="mimeType">Internet media type.</param>
        /// <param name="extension">File type extension.</param>
        /// <returns>true if the the entry could be removed.</returns>
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


        /// <summary>
        /// Tries to get the next row from the addendum, which corresponds to the <paramref name="mediaType"/>. If
        /// <paramref name="mediaType"/> is null, it tries to get the next row. If the method successfully returns,
        /// it removes <paramref name="row"/> from the addendum.
        /// </summary>
        /// <param name="mediaType">
        /// <para>The first part of an Internet media type (mediatype/subtype) or null.</para>
        /// <para>
        /// If <paramref name="mediaType"/> is null, the method tries to get the next row from the addendum and
        /// writes the media type of this <paramref name="row"/> to the parameter when the method successfully returns.
        /// </para>
        /// <para>
        /// If <paramref name="mediaType"/> is not null, the method tries to find the next row in the addendum with the specified
        /// media type.
        /// </para>
        /// </param>
        /// <param name="row">The row from the addendum if the method successfully returns, otherwise null.</param>
        /// <returns>true, if a corresponding row could be found.</returns>
        public bool TryGetLine([NotNullWhen(true)] ref string? mediaType, [NotNullWhen(true)] out AddendumRow? row)
        {
            row = null;

            if (string.IsNullOrWhiteSpace(mediaType))
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
