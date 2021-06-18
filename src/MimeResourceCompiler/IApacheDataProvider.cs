namespace MimeResourceCompiler
{
    public interface IApacheDataProvider
    {
        string? GetNextLine();
        void TestApacheFile(string mediaType);
    }
}