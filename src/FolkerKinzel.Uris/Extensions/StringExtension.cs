namespace FolkerKinzel.Uris.Extensions;

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
        => urlString is not null && urlString.StartsWith(DataUrl.Protocol, StringComparison.OrdinalIgnoreCase);
}
