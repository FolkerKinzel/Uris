using System.Diagnostics.CodeAnalysis;

namespace MimeResourceCompiler
{
    public interface IAddendum
    {
        bool GetLine([NotNullWhen(true)] ref string? mediaType, [NotNullWhen(true)] out AddendumRow? row);
        bool RemoveFromAddendum(string mimeType, string extension);
    }
}