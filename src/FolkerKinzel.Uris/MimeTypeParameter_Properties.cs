using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeTypeParameter : IEquatable<MimeTypeParameter>, ICloneable
    {
        private readonly ReadOnlyMemory<char> _parameterString;

        // Stores all indexes in one int to let the struct not grow too large:
        // |        Key Length          |        Value Start       |
        // |    Byte 4    |    Byte 3   |    Byte 2   |   Byte 1   |
        private readonly int _idx;
        
        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s key.
        /// </summary>
        public ReadOnlySpan<char> Key => _parameterString.Span.Slice(0, (_idx >> KEY_LENGTH_SHIFT) & 0xFFFF);

        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s value.
        /// </summary>
        public ReadOnlySpan<char> Value => _parameterString.Span.Slice(_idx & 0xFFFF);

        /// <summary>
        /// <c>true</c> indicates that the instance contains no data.
        /// </summary>
        public bool IsEmpty => Key.IsEmpty;

        /// <summary>
        /// Returns an empty <see cref="MimeTypeParameter"/> structure.
        /// </summary>
        public static MimeTypeParameter Empty => default;

        
        /// <summary>
        /// Determines whether the <see cref="MimeTypeParameter"/> has the <see cref="Key"/> "charset". The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if <see cref="Key"/> equals "charset"; otherwise, <c>false</c>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool IsCharsetParameter
            => Key.Equals(CHARSET_KEY.AsSpan(), StringComparison.OrdinalIgnoreCase);


        /// <summary>
        /// Determines whether this instance equals "charset=us-ascii". The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if this instance equals "charset=us-ascii"; otherwise, <c>false</c>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool IsAsciiCharsetParameter()
            => IsCharsetParameter
               && Value.Equals(ASCII_CHARSET_VALUE.AsSpan(), StringComparison.OrdinalIgnoreCase);


    }
}
