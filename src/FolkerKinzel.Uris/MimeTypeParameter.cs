using System;
using FolkerKinzel.Uris.Properties;
using System.Text;
using FolkerKinzel.Uris.Intls;
using System.Runtime.CompilerServices;
using System.Diagnostics;

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
        private const string CHARSET_KEY = "charset";
        private const string ASCII_CHARSET_VALUE = "us-ascii";

        private readonly ReadOnlyMemory<char> _parameterString;
        private readonly int _keyLength;
        private readonly int _valueStart;
        internal const int StringLength = 32;

        /// <summary>
        /// Initializes a new <see cref="MimeTypeParameter"/> structure.
        /// </summary>
        /// <param name="parameterString">The trimmed Parameter.</param>
        /// <param name="value">The length of the Key part.</param>
        /// <param name="valueStart">The start index of the Value.</param>
        private MimeTypeParameter(in ReadOnlyMemory<char> parameterString, int keyLength, int valueStart)
        {
            this._parameterString = parameterString;
            this._keyLength = keyLength;
            this._valueStart = valueStart;
        }

        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s key.
        /// </summary>
        public ReadOnlySpan<char> Key => _parameterString.Span.Slice(0, _keyLength);

        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s value.
        /// </summary>
        public ReadOnlySpan<char> Value => _parameterString.Span.Slice(_valueStart);

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

            Debug.Assert(!char.IsWhiteSpace(parameterString.Span[0]));
            Debug.Assert(!char.IsWhiteSpace(parameterString.Span[parameterString.Length - 1]));
            Debug.Assert(parameterString.Span[parameterString.Length - 1] != '"');

            int keyLength = span.Slice(0, keyValueSeparatorIndex).GetTrimmedLength();

            if (keyLength == 0)
            {
                goto Failed;
            }

            int valueStart = span.Slice(keyValueSeparatorIndex + 1).GetTrimmedStart();

            if(valueStart == span.Length)
            {
                goto Failed;
            }

            if(span[valueStart] == '"')
            {
                valueStart++;
            }

            if(valueStart == span.Length)
            {
                goto Failed;
            }


            parameter = new MimeTypeParameter(in parameterString, keyLength, valueStart);

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
