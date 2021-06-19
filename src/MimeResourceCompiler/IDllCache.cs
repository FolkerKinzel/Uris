using System.Diagnostics.CodeAnalysis;

namespace MimeResourceCompiler
{
    public interface IDllCache
    {
        bool TryGetMimeTypeFromFileTypeExtension(string extension, [NotNullWhen(true)] out string? mimeType);
    }
}