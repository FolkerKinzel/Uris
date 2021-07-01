using System.Diagnostics.CodeAnalysis;

namespace MimeResourceCompiler
{
    public interface IAddendum
    {
        /// <summary>
        /// Tries to get the next row from the addendum, which corresponds to the <paramref name="mediaType"/>. If
        /// <paramref name="mediaType"/> is null, it tries to get the next row. If the method successfully returns,
        /// it removes <paramref name="row"/> from the addendum.
        /// </summary>
        /// <param name="mediaType">
        /// <para>The first part of an Internet media type (mediatype/subtype) or null.</para>
        /// <para>
        /// If <paramref name="mediaType"/> is null, the method tries to get the next row from the addendum and
        /// writes the media type of this <paramref name="row"/> to the parameter when the method successfully returns.
        /// </para>
        /// <para>
        /// If <paramref name="mediaType"/> is not null, the method tries to find the next row in the addendum with the specified
        /// media type.
        /// </para>
        /// </param>
        /// <param name="row">The row from the addendum if the method successfully returns, otherwise null.</param>
        /// <returns>true, if a corresponding row could be found.</returns>
        bool TryGetLine([NotNullWhen(true)] ref string? mediaType, [NotNullWhen(true)] out AddendumRow? row);
        
        /// <summary>
        /// Removes an entry from the addendum.
        /// </summary>
        /// <param name="mimeType">Internet media type.</param>
        /// <param name="extension">File type extension.</param>
        /// <returns>true if the the entry could be removed.</returns>
        bool RemoveFromAddendum(string mimeType, string extension);
    }
}