using System.IO;

namespace MimeResourceCompiler
{
    public interface IStreamFactory
    {
        Stream CreateWriteStream(string fileName);
    }
}