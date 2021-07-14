using System;
using FolkerKinzel.Uris.Properties;
using System.Text;
using FolkerKinzel.Uris.Intls;

#if NETSTANDARD2_0 || NETSTANDARD2_1
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    public readonly struct MediaTypeParameter : IEquatable<MediaTypeParameter>
    {
        private readonly ReadOnlyMemory<char> _key;
        private readonly ReadOnlyMemory<char> _value;
        private const string CHARSET_KEY = "charset";

        internal const int StringLength = 32;

        public MediaTypeParameter(ReadOnlyMemory<char> key, ReadOnlyMemory<char> value)
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

        public ReadOnlySpan<char> Key => _key.Span;
        public ReadOnlySpan<char> Value => _value.Span;

        internal static bool TryParse(ReadOnlyMemory<char> parameterString, out MediaTypeParameter parameter)
        {
            int keyValueSeparatorIndex = parameterString.Span.IndexOf('=');

            if (keyValueSeparatorIndex < 1)
            {
                parameter = default;
                return false;
            }

            try
            {
                parameter = new MediaTypeParameter(
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Bedingten Ausdruck vereinfachen", Justification = "<Ausstehend>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public bool Equals(MediaTypeParameter other)
        {
            return !Key.Equals(other.Key, StringComparison.OrdinalIgnoreCase)
                ? false
                : Key.Equals(CHARSET_KEY.AsSpan(), StringComparison.OrdinalIgnoreCase)
                    ? Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase)
                    : Value.Equals(other.Value, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj) => obj is MediaTypeParameter parameter && Equals(parameter);

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

            if (Key.Equals(CHARSET_KEY.AsSpan(), StringComparison.OrdinalIgnoreCase))
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

        public static bool operator ==(MediaTypeParameter mediaTypeParameter1, MediaTypeParameter mediaTypeParameter2) => mediaTypeParameter1.Equals(mediaTypeParameter2);

        public static bool operator !=(MediaTypeParameter mediaTypeParameter1, MediaTypeParameter mediaTypeParameter2) => !(mediaTypeParameter1 == mediaTypeParameter2);


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
            if(Key.Length == 0)
            {
                return;
            }

            _ = builder.EnsureCapacity(builder.Length + StringLength);

            // RFC 2045 Section 5.1 "tspecials"
            ReadOnlySpan<char> maskChars = stackalloc char[] { ' ', '(', ')', '<', '>', '@', ',', ';', ':', '\\', '\"', '/', '[', '>', ']', '?', '=' };

            int keyStart = builder.Length;
            _ = builder.Append(Key).ToLowerInvariant(keyStart).Append('=');


            bool mask = Value.ContainsAny(maskChars);
            {
                if (mask)
                {
                    _ = builder.Append('\"');
                }

                int valueStart = builder.Length;
                _ = Key.CompareTo(CHARSET_KEY.AsSpan(), StringComparison.OrdinalIgnoreCase) == 0
                    ? builder.Append(Value).ToLowerInvariant()
                    : builder.Append(Value);

                if (mask)
                {
                    _ = builder.Append('\"');
                }
            }
        }
    }
}
