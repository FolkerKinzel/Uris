using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FolkerKinzel.Uris.Tests
{
    [TestClass]
    public class DataUrlTests
    {
        // private const string DEFAULT_MIME_TYPE = "text/plain;charset=us-ascii";
        private const string DEFAULT_ENCODING = "UrlEncoding";
        private const string DATA_PROTOCOL = "data:";

        //[TestMethod]
        //public void SchemeDelimiterTest() => Assert.AreEqual(":", DataUrl.SchemeDelimiter);

        [TestMethod]
        public void TryParseTest1()
        {
            string text = "http://www.fölkerchen.de";


            string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

            Assert.IsTrue(DataUrl.TryParse(test, out DataUrl dataUri));

            Assert.IsTrue(dataUri.TryGetEmbeddedText(out string? outText));
            Assert.AreEqual(text, outText);

            outText = DataUrl.FromText(text);

            Assert.IsNotNull(outText);

            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType mime));

            outText = DataUrl.FromBytes(new byte[] { 1, 2, 3 }, mime);

            Assert.IsNotNull(outText);
        }

        [TestMethod]
        public void TryParseTest2()
        {
            string text = "http://www.fölkerchen.de";
            string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

            string outText = DataUrl.FromText(text);

            Assert.IsNotNull(outText);

            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType mime));

            outText = DataUrl.FromBytes(new byte[] { 1, 2, 3 }, mime);

            Assert.IsNotNull(outText);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("http://wwww.folker-kinzel.de/index.htm")]
        public void TryParseTest3(string? value) => Assert.IsFalse(DataUrl.TryParse(value, out _));


        [TestMethod()]
        public void TryParseTest4()
        {
            string test = "data:;charset=UTF-8,Text";

            Assert.IsTrue(DataUrl.TryParse(test, out DataUrl dataUrl2));

            Assert.AreEqual(dataUrl2.EmbeddedData.ToString(), "Text");
            Assert.AreEqual(dataUrl2.MimeType.MediaType.ToString(), "text");
            Assert.AreEqual(dataUrl2.MimeType.SubType.ToString(), "plain");

            Assert.AreEqual(dataUrl2.MimeType.Parameters.First().Value.ToString(), "UTF-8");
            Assert.AreEqual(dataUrl2.DataEncoding, DataEncoding.UrlEncoded);

            Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outString));
            Assert.AreEqual("Text", outString);
        }


        [TestMethod]
        public void FromBytesTest1()
        {
            string text = "http://www.fölkerchen.de";
            string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

            string outText = DataUrl.FromText(text);

            Assert.IsNotNull(outText);

            
        }

        [TestMethod]
        public void FromBytesTest2()
        {
            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType mime));

            byte[] bytes = new byte[] { 1, 2, 3 };
            string outText = DataUrl.FromBytes(bytes, mime);

            Assert.IsNotNull(outText);

            Assert.IsTrue(DataUrl.TryParse(outText, out DataUrl dataUrl));

            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));

            CollectionAssert.AreEqual(bytes, outBytes);
        }


        [TestMethod]
        public void FromFileTest1()
        {
            string path = TestFiles.FolkerPng;
            string url = DataUrl.FromFile(path);
            Assert.IsNotNull(url);

            Assert.IsTrue(DataUrl.TryParse(url, out DataUrl dataUrl));

            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));

            CollectionAssert.AreEqual(outBytes, File.ReadAllBytes(path));
        }

        [TestMethod]
        public void FromTextOnNull()
        {
            Assert.IsNotNull(DataUrl.FromText(null));
        }

        [TestMethod]
        public void FromTextOnStringEmpty()
        {
            Assert.IsNotNull(DataUrl.FromText(""));
        }

        [TestMethod()]
        public void FromTextTest1()
        {
            const string TEXT = "In Märchenbüchern herumstöbern.";

            string dataUrl1 = DataUrl.FromText(Uri.EscapeDataString(TEXT));

            Assert.IsTrue(DataUrl.TryParse(dataUrl1, out DataUrl dataUrl2));

            Assert.AreEqual(dataUrl2.MimeType.MediaType.ToString(), "text");
            Assert.AreEqual(dataUrl2.MimeType.SubType.ToString(), "plain");

            Assert.AreEqual(0, dataUrl2.MimeType.Parameters.Count());

            Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outText));
            Assert.AreEqual(TEXT, outText);
        }



        [TestMethod()]
        public void FromTextTest2()
        {
            const string TEXT = "In Märchenbüchern herumstöbern.";

            string dataUrl1 = DataUrl.FromText(TEXT);

            Assert.IsTrue(DataUrl.TryParse(dataUrl1, out DataUrl dataUrl2));

            Assert.AreEqual(dataUrl2.MimeType.MediaType.ToString(), "text");
            Assert.AreEqual(dataUrl2.MimeType.SubType.ToString(), "plain");

            Assert.AreEqual(0, dataUrl2.MimeType.Parameters.Count());

            Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outText));
            Assert.AreEqual(TEXT, outText);
        }

    }
}
