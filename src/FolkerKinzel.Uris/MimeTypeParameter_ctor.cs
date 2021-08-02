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
    /// <remarks>
    /// <note type="tip">
    /// <para>
    /// <see cref="MimeTypeParameter"/> is a quite large structure. Pass it to other methods by reference (in, ref or out parameters in C#)!
    /// </para>
    /// <para>
    /// If you intend to hold a <see cref="MimeTypeParameter"/> for a long time in memory and if this <see cref="MimeTypeParameter"/> is parsed
    /// from a <see cref="ReadOnlyMemory{T}">ReadOnlyMemory&lt;Char&gt;</see> that comes from a very long <see cref="string"/>, 
    /// keep in mind, that the <see cref="MimeTypeParameter"/> holds a reference to that <see cref="string"/>. Consider in this case to make
    /// a copy of the <see cref="MimeType"/> structure with <see cref="MimeTypeParameter.Clone"/>: The copy is built on a separate <see cref="string"/>,
    /// which is case-normalized and only as long as needed.
    /// </para>
    /// </note>
    /// </remarks>
    public readonly partial struct MimeTypeParameter : IEquatable<MimeTypeParameter>, ICloneable
    {
        /// <summary>
        /// Initializes a new <see cref="MimeTypeParameter"/> structure.
        /// </summary>
        /// <param name="parameterString">The trimmed Parameter.</param>
        /// <param name="idx">All indexes in one Int32.</param>
        private MimeTypeParameter(in ReadOnlyMemory<char> parameterString, int idx)
        {
            this._parameterString = parameterString;
            this._idx = idx;
        }

        internal static bool TryParse(ref ReadOnlyMemory<char> value, out MimeTypeParameter parameter)
        {
            value = value.Trim();

            if (value.Length == 0)
            {
                goto Failed;
            }

            ReadOnlySpan<char> span = value.Span;

            if (span[span.Length - 1] == '"')
            {
                value = value.Slice(0, value.Length - 1);
                span = value.Span;
            }

            int keyValueSeparatorIndex = span.IndexOf('=');

            if (keyValueSeparatorIndex < 1)
            {
                goto Failed;
            }

            int keyLength = span.Slice(0, keyValueSeparatorIndex).GetTrimmedLength();

            if (keyLength is 0 or > short.MaxValue)
            {
                goto Failed;
            }

            int valueStart = keyValueSeparatorIndex + 1;

            if (valueStart == span.Length)
            {
                goto Failed;
            }

            valueStart += span.Slice(valueStart).GetTrimmedStart();

            if (span[valueStart] == '"')
            {
                valueStart++;
            }

            if (valueStart > ushort.MaxValue)
            {
                goto Failed;
            }

            int idx = keyLength << KEY_LENGTH_SHIFT;
            idx |= valueStart;

            parameter = new MimeTypeParameter(in value, idx);

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
        /// <remarks>This is the most performant overload of the Equals methods but unfortunately it's not CLS compliant.
        /// Use it if you can.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Bedingten Ausdruck vereinfachen", Justification = "<Ausstehend>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        [CLSCompliant(false)]
        public bool Equals(in MimeTypeParameter other)
            => !Key.Equals(other.Key, StringComparison.OrdinalIgnoreCase)
                ? false
                : IsCharsetParameter
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

            if (IsCharsetParameter)
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

            // RFC 2045 Section 5.1 "tspecials"
            ReadOnlySpan<char> maskChars = stackalloc char[] { ' ', '(', ')', '<', '>', '@', ',', ';', ':', '\\', '\"', '/', '[', '>', ']', '?', '=' };

            ReadOnlySpan<char> valueSpan = Value;
            ReadOnlySpan<char> keySpan = Key;

            bool mask = valueSpan.ContainsAny(maskChars);

            int neededCapacity = mask ? 2 + valueSpan.Length + keySpan.Length : valueSpan.Length + keySpan.Length;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);

            int keyStart = builder.Length;
            _ = builder.Append(Key).ToLowerInvariant(keyStart).Append('=');

            if (mask)
            {
                _ = builder.Append('\"');
            }

            int valueStart = builder.Length;
            _ = IsCharsetParameter
                ? builder.Append(Value).ToLowerInvariant(valueStart)
                : builder.Append(Value);

            if (mask)
            {
                _ = builder.Append('\"');
            }
        }

    }
}
