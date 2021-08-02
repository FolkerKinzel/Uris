using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace FolkerKinzel.Uris.Extensions
{
    /// <summary>
    /// Extension methods for the <see cref="Uri"/> class.
    /// </summary>
    public static class UriExtension
    {
        /// <summary>
        /// Returns <c>true</c> if the <see cref="Uri"/> passed as parameter is a "data" URL. (RFC 2397)
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to examine.</param>
        /// <returns><c>true</c> if <paramref name="uri"/> is a "data" URL. If <paramref name="uri"/> is 
        /// <c>null</c>&#160;<c>false</c> is returned.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDataUrl([NotNullWhen(true)] this Uri? uri) => uri is not null && uri.OriginalString.IsDataUrl();



    }
}
