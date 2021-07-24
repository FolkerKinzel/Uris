using System;
using System.Text.RegularExpressions;

namespace MimeResourceCompiler
{
    public class Entry : IEquatable<Entry?>
    {

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

            this.MediaType = mimeType.Substring(0, mediaTypeLength);
            this.Extension = PrepareFileTypeExtension(fileTypeExtension);

            if (Extension.Length == 0)
            {
                throw new ArgumentException(string.Format("Invalid file type extension: {0}", fileTypeExtension), nameof(fileTypeExtension));
            }
        }

        public string MimeType { get; }
        public string Extension { get; }
        public string MediaType { get; }

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
