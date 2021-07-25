using System;
using FolkerKinzel.Uris.Properties;
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
        private readonly ReadOnlyMemory<char> _key;
        private readonly ReadOnlyMemory<char> _value;

        internal const int StringLength = 32;

        /// <summary>
        /// Initializes a new <see cref="MimeTypeParameter"/> struct.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public MimeTypeParameter(ReadOnlyMemory<char> key, ReadOnlyMemory<char> value)
        {
            this._key = key.Trim();

            if (_key.Length == 0)
            {
                throw new ArgumentException(string.Format(Res.EmptyOrWhiteSpace, nameof(key)));
            }

            this._value = value.Trim();

            ReadOnlySpan<char> span = Value;
            if (span.Length > 0 && span[0] == '"')
            {
                _value = span.Length > 1 && span[span.Length - 1] == '"'
                            ? value.Slice(1, value.Length - 2)
                            : value.Slice(1);
            }

            if (_value.Length == 0)
            {
                throw new ArgumentException(string.Format(Res.EmptyOrWhiteSpace, nameof(value)));
            }
        }

        /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s key.
        /// </summary>
        public ReadOnlySpan<char> Key => _key.Span;

         /// <summary>
        /// The <see cref="MimeTypeParameter"/>'s value.
        /// </summary>
        public ReadOnlySpan<char> Value => _value.Span;

        /// <summary>
        /// <c>true</c> indicates that the instance contains no data.
        /// </summary>
        public bool IsEmpty => _key.IsEmpty;

        /// <summary>
        /// Returns an empty <see cref="MimeTypeParameter"/> struct.
        /// </summary>
        public static MimeTypeParameter Empty => default;

        /// <summary>
        /// Indicates that the <see cref="MimeTypeParameter"/> has the <see cref="Key"/> "charset".
        /// </summary>
        /// <returns><c>true</c> if <see cref="Key"/> equals "charset". The comparison is case-insensitive.</returns>
        public bool IsCharsetParameter()
            => Key.Equals(stackalloc char[] { 'c', 'h', 'a', 'r', 's', 'e', 't' }, StringComparison.OrdinalIgnoreCase);


        internal static bool TryParse(ReadOnlyMemory<char> parameterString, out MimeTypeParameter parameter)
        {
            int keyValueSeparatorIndex = parameterString.Span.IndexOf('=');

            if (keyValueSeparatorIndex < 1)
            {
                parameter = default;
                return false;
            }

            try
            {
                parameter = new MimeTypeParameter(
                    parameterString.Slice(0, keyValueSeparatorIndex),
                    parameterString.Slice(keyValueSeparatorIndex + 1));
            }
            catch (ArgumentException)
            {
                parameter = default;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if the content of <paramref name="other"/> is equal to that of the 
        /// current instance.
        /// </summary>
        /// <param name="other">A <see cref="MimeTypeParameter"/> struct to compare with.</param>
        /// <returns><c>true</c> if the content of <paramref name="other"/> is equal to that of the 
        /// current instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Bedingten Ausdruck vereinfachen", Justification = "<Ausstehend>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool Equals(MimeTypeParameter other)
            => !Key.Equals(other.Key, StringComparison.OrdinalIgnoreCase)
                ? false
                : IsCharsetParameter()
                    ? Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase)
                    : Value.Equals(other.Value, StringComparison.Ordinal);

        /// <summary>
        /// Determines whether <paramref name="obj"/> is a <see cref="MimeTypeParameter"/> struct
        /// whose content is equal to that of the current instance.
        /// </summary>
        /// <param name="obj">A <see cref="MimeTypeParameter"/> struct to compare with.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is a <see cref="MimeTypeParameter"/> struct
        /// whose content is equal to that of the current instance.</returns>
        public override bool Equals(object? obj) => obj is MimeTypeParameter parameter && Equals(parameter);

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
        /// otherwise, false.</returns>
        public static bool operator ==(MimeTypeParameter mimeTypeParameter1, MimeTypeParameter mimeTypeParameter2) => mimeTypeParameter1.Equals(mimeTypeParameter2);

        /// <summary>
        /// Returns a value that indicates whether two specified <see cref="MimeTypeParameter"/> instances are not equal.
        /// </summary>
        /// <param name="mimeTypeParameter1">The first <see cref="MimeTypeParameter"/> to compare.</param>
        /// <param name="mimeTypeParameter2">The second <see cref="MimeTypeParameter"/> to compare.</param>
        /// <returns><c>true</c> if <paramref name="mimeTypeParameter1"/> and <paramref name="mimeTypeParameter2"/> are not equal;
        /// otherwise, false.</returns>
        /// <returns></returns>
        public static bool operator !=(MimeTypeParameter mimeTypeParameter1, MimeTypeParameter mimeTypeParameter2) => !(mimeTypeParameter1 == mimeTypeParameter2);


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
            _ = builder.Append(';').Append(Key).ToLowerInvariant(keyStart).Append('=');


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
