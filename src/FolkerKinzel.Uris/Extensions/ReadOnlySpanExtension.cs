namespace FolkerKinzel.Uris.Extensions;

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
        => span.StartsWith(DataUrl.Protocol.AsSpan(), StringComparison.OrdinalIgnoreCase);

}
