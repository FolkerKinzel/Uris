using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrlInfo
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
        /// Indicates whether <see cref="Data"/> contains text.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Data"/> contains text, otherwise <c>false</c>.
        /// </value>
        public bool ContainsEmbeddedText => this.MimeType.IsText;

        /// <summary>
        /// Indicates whether <see cref="Data"/> contains binary data.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Data"/> contains binary data, otherwise <c>false</c>.
        /// </value>
        public bool ContainsEmbeddedBytes => Encoding == ContentEncoding.Base64 || !ContainsEmbeddedText;

        /// <summary>
        /// Indicates whether the instance contains no data.
        /// </summary>
        /// <value>
        /// <c>true</c> if the instance contains no data, otherwise <c>false</c>.
        /// </value>
        public bool IsEmpty => this.MimeType.IsEmpty;

        /// <summary>
        /// Returns a <see cref="DataUrlInfo"/> instance, which is <see cref="Empty"/>.
        /// </summary>
        public static DataUrlInfo Empty => default;

        #endregion

    }
}
