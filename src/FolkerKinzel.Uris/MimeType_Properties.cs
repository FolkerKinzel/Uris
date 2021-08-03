using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeType : IEquatable<MimeType>, ICloneable
    {
        private readonly ReadOnlyMemory<char> _mimeTypeString;

        // Stores all indexes in one int.
        // | TopLevlMediaTp Length |  SubType Start  |  SubType Length  |  Parameters Start  |
        // |       Byte 4          |     Byte 3      |       Byte 2     |     Byte 1         |
        private readonly int _idx;


        #region Properties

        /// <summary>
        /// Top-Level Media Type. (The left part of a MIME-Type.)
        /// </summary>
        public ReadOnlySpan<char> TopLevelMediaType => _mimeTypeString.Span.Slice(0, (_idx >> TOP_LEVEL_MEDIA_TYPE_LENGTH_SHIFT) & 0xFF);

        /// <summary>
        /// Sub Type (The right part of a MIME-Type.)
        /// </summary>
        public ReadOnlySpan<char> SubType => _mimeTypeString.Span.Slice((_idx >> SUB_TYPE_START_SHIFT) & 0xFF, (_idx >> SUB_TYPE_LENGTH_SHIFT) & 0xFF);

        /// <summary>
        /// Parameters (Never <c>null</c>.)
        /// </summary>
        public IEnumerable<MimeTypeParameter> Parameters => ParseParameters();

        /// <summary>
        /// <c>true</c> if the instance contains no data.
        /// </summary>
        public bool IsEmpty => TopLevelMediaType.IsEmpty;

        /// <summary>
        /// Returns a <see cref="MimeType"/> structure, which contains no data.
        /// </summary>
        public static MimeType Empty => default;

        /// <summary>
        /// Finds an appropriate file type extension for the <see cref="MimeType"/> instance.
        /// </summary>
        /// <returns>An appropriate file type extension for the <see cref="MimeType"/> instance.</returns>
        public string GetFileTypeExtension()
            => MimeCache.GetFileTypeExtension(ToString(false));

        /// <summary>
        /// Determines whether the <see cref="TopLevelMediaType"/> of this instance equals "text".
        /// The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="TopLevelMediaType"/> of this instance equals "text".</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal bool IsText
            => TopLevelMediaType.Equals("text".AsSpan(), StringComparison.OrdinalIgnoreCase);


        /// <summary>
        /// Determines whether this instance is equal to the MIME type "text/plain". The parameters are not taken into account.
        /// The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if this instance is equal to "text/plain".</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal bool IsTextPlain
            => IsText && SubType.Equals("plain".AsSpan(), StringComparison.OrdinalIgnoreCase);

        

        #endregion

    }
}
