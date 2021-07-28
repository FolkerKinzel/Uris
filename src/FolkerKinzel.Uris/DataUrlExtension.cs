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
    /// <summary>
    /// Extension methods, which support the <see cref="DataUrl"/> struct.
    /// </summary>
    public static class DataUrlExtension
    {

        /// <summary>
        /// Returns <c>true</c> if the <see cref="Uri"/> passed as parameter is a "data" URL. (RFC 2397)
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to examine.</param>
        /// <returns><c>true</c> if <paramref name="uri"/> is a "data" URL. If <paramref name="uri"/> is 
        /// <c>null</c>&#160;<c>false</c> is returned.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDataUrl([NotNullWhen(true)] this Uri? uri) => uri is not null && uri.OriginalString.IsDataUrl();

        /// <summary>
        /// Returns <c>true</c> if the <see cref="string"/> passed as parameter is a "data" URL. (RFC 2397)
        /// </summary>
        /// <param name="urlString">The <see cref="string"/> to examine.</param>
        /// <returns><c>true</c> if <paramref name="urlString"/> is a "data" URL. If <paramref name="urlString"/> is 
        /// <c>null</c> or empty <c>false</c> is returned.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDataUrl([NotNullWhen(true)] this string? urlString)
            => urlString is not null && urlString.StartsWith(DataUrl.PROTOCOL, StringComparison.OrdinalIgnoreCase);


        internal static StringBuilder AppendMediaType(this StringBuilder builder, MimeType mediaType)
        {
            if (mediaType.IsEmpty || mediaType.Equals(DataUrl.DefaultMediaType()))
            {
                return builder;
            }

            if (mediaType.IsTextPlain())
            {
                foreach (MimeTypeParameter parameter in mediaType.Parameters)
                {
                    parameter.AppendTo(builder);
                }

                return builder;
            }

            return mediaType.AppendTo(builder);
        }

    }
}
