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

namespace FolkerKinzel.Uris.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="string"/> class.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Returns <c>true</c> if the <see cref="string"/> passed as parameter is a "data" URL. (RFC 2397)
        /// </summary>
        /// <param name="urlString">The <see cref="string"/> to examine.</param>
        /// <returns><c>true</c> if <paramref name="urlString"/> is a "data" URL. If <paramref name="urlString"/> is 
        /// <c>null</c> or empty <c>false</c> is returned.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDataUrl([NotNullWhen(true)] this string? urlString)
            => urlString is not null && urlString.StartsWith(DataUrl.PROTOCOL, StringComparison.OrdinalIgnoreCase);


    }
}
