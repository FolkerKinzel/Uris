namespace FolkerKinzel.Uris.Tests;

[TestClass]
public class DataUrlBuilderTests
{

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    public TestContext TestContext { get; set; }
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.


    [TestMethod]
    public void FromFileTest1()
    {
        const string fileName = "test.jpg";
        byte[] testData = new byte[] { 1, 2, 3 };
        string path = Path.Combine(TestContext.TestRunDirectory, fileName);
        File.WriteAllBytes(path, testData);

        string url1 = DataUrlBuilder.FromFile(path);
        StringAssert.Contains(url1, "image/jpeg");
    }

    [TestMethod]
    public void FromFileTest2()
    {
        const string fileName = "test.jpg";
        byte[] testData = new byte[] { 1, 2, 3 };
        string path = Path.Combine(TestContext.TestRunDirectory, fileName);
        File.WriteAllBytes(path, testData);

        MimeType mime = MimeType.Parse("image/png");

        string url1 = DataUrlBuilder.FromFile(path, mime);
        StringAssert.Contains(url1, "image/png");
    }
}
