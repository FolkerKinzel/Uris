using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.URIs.Intls.Tests
{
    [TestClass]
    public class MimeCacheTests
    {
        [DataTestMethod]
        [DataRow(".json", "application/json")]
        [DataRow(".psd", "image/vnd.adobe.photoshop")]
        [DataRow(".json", "application/json")]
        public void GetMimeTypeTest1(string extension, string mimeType)
        {
           Assert.AreEqual(mimeType, MimeCache.GetMimeType(extension, 5), true);
        }



        [DataTestMethod]
        //[DataRow(".ez", "application/andrew-inset")]
        [DataRow(".ice", "x-conference/x-cooltalk")]
        [DataRow(".ttc", "font/collection")]
        [DataRow(".woff2", "font/woff2")]
        [DataRow(".bin", "font/blabla")]
        [DataRow(".json", "application/json")]
        [DataRow(".psd", "image/vnd.adobe.photoshop")]
        [DataRow(".json", "application/json")]
        public void GetFileTypeExtensionTest1(string extension, string mimeType)
        {
           Assert.AreEqual(extension,  MimeCache.GetFileTypeExtension(mimeType, 5), true);
        }


        
    }
}
