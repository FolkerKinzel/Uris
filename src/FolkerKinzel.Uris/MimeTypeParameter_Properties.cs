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

        // Stores all indexes in 2 int to let the struct not grow too large:
        // |  Key Length  | CharsetStart  |         Value Start           |
        // |    Byte 4    |     Byte 3    |      Byte 2    |   Byte 1     |
        private readonly int _idx1;

        // | LanguageLength |        LanguageStart          | CharsetLength |
        // |      Byte 4    |    Byte 3     |     Byte 2    |     Byte 1    |
        private readonly int _idx2;

        private const int LANGUAGE_LENGTH_SHIFT = 24;
        private const int LANGUAGE_START_SHIFT = 8;
        private const int KEY_LENGTH_SHIFT = 24;
        private const int CHARSET_START_SHIFT = 16;

        private const int KEY_LENGTH_MAX_VALUE = sbyte.MaxValue;

        private const int LANGUAGE_LENGTH_MAX_VALUE = sbyte.MaxValue;
        private const int LANGUAGE_START_MAX_VALUE = ushort.MaxValue;
        private const int CHARSET_START_MAX_VALUE = byte.MaxValue;
        private const int CHARSET_LENGTH_MAX_VALUE = byte.MaxValue;

        private const int VALUE_START_MAX_VALUE = ushort.MaxValue;

        
        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s key.
        /// </summary>
        public ReadOnlySpan<char> Key => _parameterString.Span.Slice(0, (_idx1 >> KEY_LENGTH_SHIFT) & KEY_LENGTH_MAX_VALUE);

        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s value.
        /// </summary>
        public ReadOnlySpan<char> Value => _parameterString.Span.Slice(_idx1 & VALUE_START_MAX_VALUE);

        public ReadOnlySpan<char> Language => _parameterString.Span.Slice(
            (_idx2 >> LANGUAGE_START_SHIFT) & LANGUAGE_START_MAX_VALUE, (_idx2 >> LANGUAGE_LENGTH_SHIFT) & LANGUAGE_LENGTH_MAX_VALUE);

        public ReadOnlySpan<char> Charset => _parameterString.Span.Slice(
            (_idx1 >> CHARSET_START_SHIFT) & CHARSET_START_MAX_VALUE, _idx2 & CHARSET_LENGTH_MAX_VALUE);

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
        /// Determines whether the <see cref="MimeTypeParameter"/> has the <see cref="Key"/> "access-type". The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if <see cref="Key"/> equals "access-type"; otherwise, <c>false</c>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool IsAccessTypeParameter
            => Key.Equals("access-type".AsSpan(), StringComparison.OrdinalIgnoreCase);

        private bool IsValueCaseSensitive => !(IsCharsetParameter || IsAccessTypeParameter);

        /// <summary>
        /// Determines whether this instance equals "charset=us-ascii". The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if this instance equals "charset=us-ascii"; otherwise, <c>false</c>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal bool IsAsciiCharsetParameter()
            => IsCharsetParameter
               && Value.Equals(ASCII_CHARSET_VALUE.AsSpan(), StringComparison.OrdinalIgnoreCase);


    }
}
