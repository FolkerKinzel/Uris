namespace MimeResourceCompiler
{
    public interface IApacheData
    {
        string? GetNextLine();
        void TestApacheFile(string mediaType);
    }
}