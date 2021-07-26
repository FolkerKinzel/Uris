using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris.Intls
{
    internal static class HelperExtension
    {
        internal static StringBuilder ToLowerInvariant(this StringBuilder builder)
            => builder.ToLowerInvariant(0, builder.Length);


        internal static StringBuilder ToLowerInvariant(this StringBuilder builder, int startIndex)
            => builder.ToLowerInvariant(startIndex, builder.Length - startIndex);


        internal static StringBuilder ToLowerInvariant(this StringBuilder builder, int startIndex, int count)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            count += startIndex;
            for (int i = startIndex; i < count; i++)
            {
                char current = builder[i];
                if(char.IsUpper(current))
                {
                    builder[i] = char.ToLowerInvariant(current);
                }
            }

            return builder;
        }


        internal static bool ContainsAny(this ReadOnlySpan<char> span, ReadOnlySpan<char> chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                if(span.Contains(chars[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
