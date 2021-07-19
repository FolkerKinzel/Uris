using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#if NETSTANDARD2_0 || NET461
using FolkerKinzel.Strings.Polyfills;
#endif

namespace FolkerKinzel.Uris
{
    public static class DataUrlExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDataUrl([NotNullWhen(true)] this string? urlString) => urlString.StartsWithDataUrlProtocol();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDataUrl([NotNullWhen(true)] this Uri? dataUrl) => dataUrl is not null && dataUrl.OriginalString.IsDataUrl();

        internal static bool StartsWithDataUrlProtocol([NotNullWhen(true)] this string? input) 
            => input is not null
               && input.AsSpan().StartsWith(stackalloc char[] { 'd', 'a', 't', 'a', ':' }, StringComparison.OrdinalIgnoreCase);

        internal static StringBuilder AppendProtocol(this StringBuilder sb)
            => sb.Append(stackalloc char[] { 'd', 'a', 't', 'a', ':' });

        internal static StringBuilder AppendMediaType(this StringBuilder builder, MimeType mediaType)
        {
            if (mediaType.IsEmpty || mediaType.Equals(DataUrl.DefaultMediaType()))
            {
                return builder;
            }

            if (mediaType.IsTextPlainType())
            {
                foreach (MimeTypeParameter parameter in mediaType.Parameters)
                {
                    parameter.AppendTo(builder);
                }

                return builder;
            }

            return mediaType.AppendTo(builder);
        }


        internal static StringBuilder AppendBase64(this StringBuilder builder) 
            => builder.Append(stackalloc char[] { ';', 'b', 'a', 's', 'e', '6', '4' });
    }
}
