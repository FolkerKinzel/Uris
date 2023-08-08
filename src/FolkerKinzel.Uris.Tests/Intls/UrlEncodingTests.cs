using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace FolkerKinzel.Uris.Intls.Tests;

[TestClass]
public class UrlEncodingTests
{
    [TestMethod]
    public void EncodeBytesTest()
    {
        byte[] bytes = new byte[] { 131, 142, 175 };
        string s = UrlEncoding.EncodeBytes(bytes);
        StringAssert.Contains(s, "%");

    }
}
