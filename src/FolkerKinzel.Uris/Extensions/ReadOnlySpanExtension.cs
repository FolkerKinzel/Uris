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
    /// Extension methods for the <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;Char&gt;</see> structure.
    /// </summary>
    public static class ReadOnlySpanExtension
    {
        /// <summary>
        /// Returns <c>true</c> if the <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;Char&gt;</see> passed as parameter is a "data" URL. (RFC 2397)
        /// </summary>
        /// <param name="span">The <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;Char&gt;</see> to examine.</param>
        /// <returns><c>true</c> if <paramref name="span"/> contains a "data" URL.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Globalization", "CA1303:Literale nicht als lokalisierte Parameter übergeben", Justification = "<Ausstehend>")]
        public static bool IsDataUrl(this ReadOnlySpan<char> span)
            => span.StartsWith(DataUrl.PROTOCOL.AsSpan(), StringComparison.OrdinalIgnoreCase);



    }
}
