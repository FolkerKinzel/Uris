using System;

namespace FolkerKinzel.Uris.Intls
{
    internal static class TrimHelper
    {
        internal static ReadOnlyMemory<char> Trim(in ReadOnlyMemory<char> value)
        {
            ReadOnlySpan<char> span = value.Span;

            int trimmedStart = span.GetTrimmedStart();
            int trimmedLength = span.Length - trimmedStart;
            trimmedLength = span.Slice(trimmedStart, trimmedLength).GetTrimmedLength();

            return trimmedLength == span.Length ? value : value.Slice(trimmedStart, trimmedLength);
        }

        internal static ReadOnlyMemory<char> TrimStart(in ReadOnlyMemory<char> value)
        {
            ReadOnlySpan<char> span = value.Span;
            int trimmedStart = span.GetTrimmedStart();
            return trimmedStart == 0 ? value : value.Slice(trimmedStart);

        }
    }
}
