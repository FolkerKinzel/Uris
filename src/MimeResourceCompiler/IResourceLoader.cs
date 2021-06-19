using System.IO;

namespace MimeResourceCompiler
{
    public interface IResourceLoader
    {
        byte[] LoadReadmeFile();
        public Stream GetAddendumStream();

    }
}