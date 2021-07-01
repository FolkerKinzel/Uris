using System.Diagnostics.CodeAnalysis;

namespace MimeResourceCompiler
{
    /// <summary>
    /// Represents the prefilled cache from the FolkerKinzel.Uris DLL.
    /// </summary>
    public interface IDllCache
    {
        /// <summary>
        /// Tries to get the Internet media type for a given file type extension.
        /// </summary>
        /// <param name="extension">The file type extension.</param>
        /// <param name="mimeType">The corresponding Internet media type if the method successfully returns, otherwise null.</param>
        /// <returns>True, if the cache had an entry for <paramref name="extension"/>.</returns>
        bool TryGetMimeTypeFromFileTypeExtension(string extension, [NotNullWhen(true)] out string? mimeType);
    }
}