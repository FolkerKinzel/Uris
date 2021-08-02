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


    }
}
