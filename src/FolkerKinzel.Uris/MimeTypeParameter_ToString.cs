using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.Uris.Intls;

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    public readonly partial struct MimeTypeParameter : IEquatable<MimeTypeParameter>, ICloneable
    {
        /// <summary>
        /// Creates a <see cref="string"/> representation of the instance.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of the instance.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder(StringLength);
            return AppendTo(sb).ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        internal StringBuilder AppendTo(StringBuilder builder, bool urlEncodedValues = false)
        {
            // Standard ctor
            if (IsEmpty)
            {
                return builder;
            }


            ReadOnlySpan<char> valueSpan = Value;
            ReadOnlySpan<char> keySpan = Key;

            // RFC 2045 Section 5.1 "tspecials"
            bool mask = valueSpan.ContainsAny(stackalloc char[] { ' ', '(', ')', '<', '>', '@', ',', ';', ':', '\\', '\"', '/', '[', '>', ']', '?', '=' });

            if (mask)
            {
                if (urlEncodedValues)
                {
                    valueSpan = Uri.EscapeDataString(valueSpan.ToString()).AsSpan();
                    mask = false;
                }
                else if (valueSpan.ContainsAny(stackalloc char[] { '"', '\\' }))
                {
                    var sb = new StringBuilder(valueSpan.Length * 2);
                    _ = sb.Append(valueSpan);
                    valueSpan = Mask(sb).ToString().AsSpan();
                }
            }

            int neededCapacity = mask ? 2 + valueSpan.Length + keySpan.Length + 1 : valueSpan.Length + keySpan.Length + 1;
            _ = builder.EnsureCapacity(builder.Length + neededCapacity);

            int keyStart = builder.Length;
            _ = builder.Append(Key).ToLowerInvariant(keyStart).Append('=');

            if (mask)
            {
                _ = builder.Append('\"');
            }

            int valueStart = builder.Length;
            _ = IsValueCaseSensitive
                ? builder.Append(Value)
                : builder.Append(Value).ToLowerInvariant(valueStart);


            if (mask)
            {
                _ = builder.Append('\"');
            }


            return builder;
        }

        private static StringBuilder Mask(StringBuilder sb)
        {
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                char current = sb[i];

                if (current is '"' or '\\')
                {
                    _ = sb.Insert(i, '\\');
                }
            }

            return sb;
        }
    }
}
