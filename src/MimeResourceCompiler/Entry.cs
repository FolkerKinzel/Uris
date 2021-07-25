using System;
using System.Text.RegularExpressions;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Encapsulates the data of one entry im Mime.csv.
    /// </summary>
    public class Entry : IEquatable<Entry?>
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="mimeType">A MIME type ("Internet media type").</param>
        /// <param name="fileTypeExtension">An appropriate file type extension for <paramref name="mimeType"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="mimeType"/> or <paramref name="fileTypeExtension"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="mimeType"/> is not a valid Internet Media Type or
        /// <paramref name="fileTypeExtension"/> is empty or consists only of whitespace.</exception>
        public Entry(string mimeType, string fileTypeExtension)
        {
            if (mimeType is null)
            {
                throw new ArgumentNullException(nameof(mimeType));
            }

            if (fileTypeExtension is null)
            {
                throw new ArgumentNullException(nameof(fileTypeExtension));
            }

            this.MimeType = PrepareMimeType(mimeType);
            int mediaTypeLength = MimeType.IndexOf('/');

            if (mediaTypeLength < 1)
            {
                throw new ArgumentException(string.Format("Invalid MIME type: {0}", mimeType), nameof(mimeType));
            }

            this.TopLevelMediaType = mimeType.Substring(0, mediaTypeLength);
            this.Extension = PrepareFileTypeExtension(fileTypeExtension);

            if (Extension.Length == 0)
            {
                throw new ArgumentException(string.Format("Invalid file type extension: {0}", fileTypeExtension), nameof(fileTypeExtension));
            }
        }

        /// <summary>
        /// Internet media type - The left part of the entry (Format: mediaType/SubType).
        /// </summary>
        public string MimeType { get; }

        /// <summary>
        /// The file type extension.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// The left part of <see cref="MimeType"/>. Used to order the entries.
        /// </summary>
        public string TopLevelMediaType { get; }

        private static string PrepareMimeType(string mimeType)
            => Regex.Replace(mimeType, @"\s+", "").ToLowerInvariant();

        private static string PrepareFileTypeExtension(string fileTypeExtension)
            => Regex.Replace(fileTypeExtension, @"\s+", "").Replace(".", null, StringComparison.Ordinal).ToLowerInvariant();

        public override string ToString() => $"{MimeType} {Extension}";

        public override bool Equals(object? obj) => obj is Entry other && Equals(other);

        public bool Equals(Entry? other)
            => other is not null 
                && StringComparer.Ordinal.Equals(MimeType, other.MimeType) 
                && StringComparer.Ordinal.Equals(Extension, other.Extension);

        public override int GetHashCode() => HashCode.Combine(MimeType, Extension);
    }
}
