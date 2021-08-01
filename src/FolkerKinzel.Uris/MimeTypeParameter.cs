using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using FolkerKinzel.Uris.Intls;

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    /// <summary>
    /// Encapsulates a parameter of a <see cref="MimeType"/>.
    /// </summary>
    public readonly struct MimeTypeParameter : IEquatable<MimeTypeParameter>
    {
        internal const int StringLength = 32;

        private const string CHARSET_KEY = "charset";
        private const string ASCII_CHARSET_VALUE = "us-ascii";

        private const int KEY_LENGTH_SHIFT = 8;
        private const int VALUE_START_SHIFT = 16;
        private const int VALUE_LENGTH_SHIFT = 24;

        private readonly ReadOnlyMemory<char> _parameterString;

        // Stores all indexes in one uint to let the struct not grow too large:
        // | Value Length | Value Start | Key Length | Key Start |
        // |    Byte 4    |    Byte 3   |   Byte 2   |   Byte 1  |
        private readonly uint _idx;

        /// <summary>
        /// Initializes a new <see cref="MimeTypeParameter"/> structure.
        /// </summary>
        /// <param name="parameterString">The trimmed Parameter.</param>
        /// <param name="value">The length of the Key part.</param>
        /// <param name="valueStart">The start index of the Value.</param>
        private MimeTypeParameter(in ReadOnlyMemory<char> parameterString, uint idx)
        {
            this._parameterString = parameterString;
            this._idx = idx;
        }

        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s key.
        /// </summary>
        public ReadOnlySpan<char> Key => _parameterString.Span.Slice((int)(_idx & 0xFF), (int)((_idx >> KEY_LENGTH_SHIFT) & 0xFF));

        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s value.
        /// </summary>
        public ReadOnlySpan<char> Value => _parameterString.Span.Slice((int)((_idx >> VALUE_START_SHIFT) & 0xFF), (int)((_idx >> VALUE_LENGTH_SHIFT) & 0xFF));

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
        public bool IsCharsetParameter()
            => Key.Equals(CHARSET_KEY.AsSpan(), StringComparison.OrdinalIgnoreCase);


        /// <summary>
        /// Determines whether this instance equals "charset=us-ascii". The comparison is case-insensitive.
        /// </summary>
        /// <returns><c>true</c> if this instance equals "charset=us-ascii"; otherwise, <c>false</c>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool IsAsciiCharsetParameter()
            => IsCharsetParameter()
               && Value.Equals(ASCII_CHARSET_VALUE.AsSpan(), StringComparison.OrdinalIgnoreCase);


        internal static bool TryParse(in ReadOnlyMemory<char> parameterString, out MimeTypeParameter parameter)
        {
            ReadOnlySpan<char> span = parameterString.Span;
            int keyValueSeparatorIndex = span.IndexOf('=');

            if (keyValueSeparatorIndex < 1)
            {
                goto Failed;
            }

            int keyStart = span.GetTrimmedStart();
            int keyLength = span.Slice(keyStart, keyValueSeparatorIndex - keyStart).GetTrimmedLength();

            if (keyLength == 0)
            {
                goto Failed;
            }

            int valueStart = keyValueSeparatorIndex + 1;

            if(valueStart == span.Length)
            {
                goto Failed;
            }

            valueStart += span.Slice(valueStart).GetTrimmedStart();


            if (span[valueStart] == '"')
            {
                valueStart++;
            }

            if (valueStart > byte.MaxValue)
            {
                goto Failed;
            }

            int valueLength = span.Slice(valueStart).GetTrimmedLength();

            if(span[valueStart + valueLength - 1].Equals('"'))
            {
                --valueLength;
            }

            if(valueLength is 0 or > byte.MaxValue)
            {
                goto Failed;
            }

            uint idx = (uint)valueLength << VALUE_LENGTH_SHIFT;
            idx |= (uint)valueStart << VALUE_START_SHIFT;
            idx |= (uint)keyLength << KEY_LENGTH_SHIFT;
            idx |= (uint)keyStart;

            parameter = new MimeTypeParameter(in parameterString, idx);

            return true;
///////////////////////////////
Failed:
            parameter = default;
            return false;
        }

        /// <summary>
        /// Determines if the content of <paramref name="other"/> is equal to that of the 
        /// current instance.
        /// </summary>
        /// <param name="other">A <see cref="MimeTypeParameter"/> structure to compare with.</param>
        /// <returns><c>true</c> if the content of <paramref name="other"/> is equal to that of the 
        /// current instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(MimeTypeParameter other) => Equals(in other);


        /// <summary>
        /// Determines if the content of <paramref name="other"/> is equal to that of the 
        /// current instance.
        /// </summary>
        /// <param name="other">A <see cref="MimeTypeParameter"/> structure to compare with.</param>
        /// <returns><c>true</c> if the content of <paramref name="other"/> is equal to that of the 
        /// current instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Bedingten Ausdruck vereinfachen", Justification = "<Ausstehend>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        [CLSCompliant(false)]
        public bool Equals(in MimeTypeParameter other)
            => !Key.Equals(other.Key, StringComparison.OrdinalIgnoreCase)
                ? false
                : IsCharsetParameter()
                    ? Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase)
                    : Value.Equals(other.Value, StringComparison.Ordinal);

        /// <summary>
        /// Determines whether <paramref name="obj"/> is a <see cref="MimeTypeParameter"/> structure
        /// whose content is equal to that of the current instance.
        /// </summary>
        /// <param name="obj">A <see cref="MimeTypeParameter"/> structure to compare with.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="MimeTypeParameter"/> structure
        /// whose content is equal to that of the current instance.</returns>
        public override bool Equals(object? obj) => obj is MimeTypeParameter parameter && Equals(in parameter);

        /// <summary>
        /// Computes a hash code for the instance.
        /// </summary>
        /// <returns>The hash code for the instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public override int GetHashCode()
        {
            var hash = new HashCode();

            ReadOnlySpan<char> keySpan = Key;
            for (int i = 0; i < keySpan.Length; i++)
            {
                hash.Add(char.ToLowerInvariant(keySpan[i]));
            }

            ReadOnlySpan<char> valueSpan = Value;

            if (IsCharsetParameter())
            {
                for (int j = 0; j < valueSpan.Length; j++)
                {
                    hash.Add(char.ToLowerInvariant(valueSpan[j]));
                }
            }
            else
            {
                for (int j = 0; j < valueSpan.Length; j++)
                {
                    hash.Add(valueSpan[j]);
                }
            }

            return hash.ToHashCode();
        }

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="MimeTypeParameter"/> instances are equal.
        /// </summary>
        /// <param name="mimeTypeParameter1">The first <see cref="MimeTypeParameter"/> to compare.</param>
        /// <param name="mimeTypeParameter2">The second <see cref="MimeTypeParameter"/> to compare.</param>
        /// <returns><c>true</c> if <paramref name="mimeTypeParameter1"/> and <paramref name="mimeTypeParameter2"/> are equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(MimeTypeParameter mimeTypeParameter1, MimeTypeParameter mimeTypeParameter2) => mimeTypeParameter1.Equals(in mimeTypeParameter2);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="MimeTypeParameter"/> instances are not equal.
        /// </summary>
        /// <param name="mimeTypeParameter1">The first <see cref="MimeTypeParameter"/> to compare.</param>
        /// <param name="mimeTypeParameter2">The second <see cref="MimeTypeParameter"/> to compare.</param>
        /// <returns><c>true</c> if <paramref name="mimeTypeParameter1"/> and <paramref name="mimeTypeParameter2"/> are not equal;
        /// otherwise, <c>false</c>.</returns>
        /// <returns></returns>
        public static bool operator !=(MimeTypeParameter mimeTypeParameter1, MimeTypeParameter mimeTypeParameter2) => !mimeTypeParameter1.Equals(in mimeTypeParameter2);


        /// <summary>
        /// Creates a <see cref="string"/> representation of the instance.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of the instance.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder(StringLength);
            AppendTo(sb);
            return sb.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal void AppendTo(StringBuilder builder)
        {
            // Standard ctor
            if (IsEmpty)
            {
                return;
            }

            _ = builder.EnsureCapacity(builder.Length + StringLength);

            // RFC 2045 Section 5.1 "tspecials"
            ReadOnlySpan<char> maskChars = stackalloc char[] { ' ', '(', ')', '<', '>', '@', ',', ';', ':', '\\', '\"', '/', '[', '>', ']', '?', '=' };

            int keyStart = builder.Length;
            _ = builder.Append(';').Append(Key).ToLowerInvariant(keyStart + 1).Append('=');


            bool mask = Value.ContainsAny(maskChars);

            if (mask)
            {
                _ = builder.Append('\"');
            }

            int valueStart = builder.Length;
            _ = IsCharsetParameter()
                ? builder.Append(Value).ToLowerInvariant(valueStart)
                : builder.Append(Value);

            if (mask)
            {
                _ = builder.Append('\"');
            }
        }
    }
}
