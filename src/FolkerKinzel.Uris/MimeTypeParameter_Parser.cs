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
        internal static bool TryParse(ref ReadOnlyMemory<char> value, out MimeTypeParameter parameter)
        {
            value = value.Trim();

            if (value.Length == 0)
            {
                goto Failed;
            }

            ReadOnlySpan<char> span = value.Span;

            int keyValueSeparatorIndex = span.IndexOf('=');

            if (keyValueSeparatorIndex < 1)
            {
                goto Failed;
            }

            // Masked Value:
            if (span[span.Length - 1] == '"')
            {
                var builder = new StringBuilder(value.Length);
                int startOfValue = keyValueSeparatorIndex + 1;
                _ = builder.Append(value).Remove(builder.Length - 1, 1);

                UnMask(builder, startOfValue);

                ReadOnlyMemory<char> mem = builder.ToString().AsMemory();
                return TryParse(ref mem, out parameter);
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

            //if (span[valueStart] == '"')
            //{
            //    valueStart++;
            //}
            //else
            //{
            //    goto Failed;
            //}

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

        private static void UnMask(StringBuilder builder, int startOfValue)
        {
            bool quotesRemoved = false;
            for (int i = startOfValue; i < builder.Length; i++)
            {
                char c = builder[i];

                if (!quotesRemoved)
                {
                    if (char.IsWhiteSpace(c))
                    {
                        _ = builder.Remove(i--, 1);
                        continue;
                    }
                    else if (c == '"')
                    {
                        _ = builder.Remove(i--, 1);
                        quotesRemoved = true;
                    }
                    else
                    {
                        quotesRemoved = true;
                    }
                }

                if (c == '\\')
                {
                    // after the mask char one entry can be skipped:
                    _ = builder.Remove(i, 1);
                }
            }
        }
    }
}
