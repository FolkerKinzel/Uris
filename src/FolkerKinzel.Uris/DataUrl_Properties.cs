using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct DataUrl : IEquatable<DataUrl>, ICloneable
    {
        private readonly ReadOnlyMemory<char> _embeddedData;

        #region Properties

        /// <summary>
        /// The internet media type of the embedded data.
        /// </summary>
        public MimeType MimeType { get; }

        /// <summary>
        /// The encoding of the data in <see cref="EmbeddedData"/>.
        /// </summary>
        public DataEncoding DataEncoding { get; }

        /// <summary>
        /// The part of the "data" URL, which contains the embedded data.
        /// </summary>
        public ReadOnlySpan<char> EmbeddedData => _embeddedData.Span;

        /// <summary>
        /// <c>true</c> if <see cref="EmbeddedData"/> contains text.
        /// </summary>
        public bool ContainsText => this.MimeType.IsText();

        /// <summary>
        /// <c>true</c> if <see cref="EmbeddedData"/> contains binary data.
        /// </summary>
        public bool ContainsBytes => DataEncoding == DataEncoding.Base64 || !ContainsText;

        /// <summary>
        /// <c>true</c> if the <see cref="DataUrl"/> contains nothing.
        /// </summary>
        public bool IsEmpty => this.MimeType.IsEmpty;

        /// <summary>
        /// Returns an empty <see cref="DataUrl"/>.
        /// </summary>
        public static DataUrl Empty => default;

        #endregion

    }
}
