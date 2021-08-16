using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrlInfo : IEquatable<DataUrlInfo>, ICloneable
    {
        private readonly ReadOnlyMemory<char> _embeddedData;
        private readonly MimeType _mimeType;

        #region Properties

        /// <summary>
        /// The internet media type of the embedded data.
        /// </summary>
        public MimeType MimeType => _mimeType;

        /// <summary>
        /// The encoding of the data in <see cref="Data"/>.
        /// </summary>
        public ContentEncoding Encoding { get; }

        /// <summary>
        /// The part of the "data" URL, which contains the embedded data.
        /// </summary>
        public ReadOnlySpan<char> Data => _embeddedData.Span;

        /// <summary>
        /// <c>true</c> if <see cref="Data"/> contains text.
        /// </summary>
        public bool ContainsEmbeddedText => this.MimeType.IsText;

        /// <summary>
        /// <c>true</c> if <see cref="Data"/> contains binary data.
        /// </summary>
        public bool ContainsEmbeddedBytes => Encoding == ContentEncoding.Base64 || !ContainsEmbeddedText;

        /// <summary>
        /// <c>true</c> if the <see cref="DataUrlInfo"/> contains nothing.
        /// </summary>
        public bool IsEmpty => this.MimeType.IsEmpty;

        /// <summary>
        /// Returns an empty <see cref="DataUrlInfo"/>.
        /// </summary>
        public static DataUrlInfo Empty => default;

        #endregion

    }
}
