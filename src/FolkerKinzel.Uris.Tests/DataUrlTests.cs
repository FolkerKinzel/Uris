﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void IsEmptyTest1() => Assert.IsTrue(DataUrl.Empty.IsEmpty);

        [TestMethod]
        public void IsEmptyTest2()
        {
            _ = DataUrl.TryParse("data:,abc", out DataUrl dataUrl);
            Assert.IsFalse(dataUrl.IsEmpty);
        }

        [TestMethod]
        public void TryParseTest1()
        {
            string text = "http://www.fölkerchen.de";


            string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

            Assert.IsTrue(DataUrl.TryParse(test, out DataUrl dataUri));

            Assert.IsTrue(dataUri.TryGetEmbeddedText(out string? outText));
            Assert.AreEqual(text, outText);

            outText = DataUrl.CreateFromText(text);

            Assert.IsNotNull(outText);

            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType mime));

            outText = DataUrl.CreateFromBytes(new byte[] { 1, 2, 3 }, mime);

            Assert.IsNotNull(outText);
        }

        [TestMethod]
        public void TryParseTest2()
        {
            string text = "http://www.fölkerchen.de";
            //string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

            string outText = DataUrl.CreateFromText(text);

            Assert.IsNotNull(outText);

            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType mime));

            outText = DataUrl.CreateFromBytes(new byte[] { 1, 2, 3 }, mime);

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
            Assert.AreEqual(dataUrl2.MimeType.TopLevelMediaType.ToString(), "text");
            Assert.AreEqual(dataUrl2.MimeType.SubType.ToString(), "plain");

            Assert.AreEqual(dataUrl2.MimeType.Parameters.First().Value.ToString(), "UTF-8");
            Assert.AreEqual(dataUrl2.DataEncoding, DataEncoding.UrlEncoded);

            Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outString));
            Assert.AreEqual("Text", outString);
        }

        [TestMethod]
        public void TryParseTest5()
        {
            const string url = "data:application/x-octet,A%42C";
            byte[] data = new byte[] { 0x41, 0x42, 0x43 };

            Assert.IsTrue(DataUrl.TryParse(url, out DataUrl dataUrl));
            Assert.AreEqual(DataEncoding.UrlEncoded, dataUrl.DataEncoding);
            Assert.IsTrue(dataUrl.ContainsBytes);

            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? output));

            CollectionAssert.AreEqual(data, output);
        }

        [DataTestMethod]
        [DataRow("data:abc")]
        //[DataRow("data:,a bc")]
        public void TryParseTest7(string input)
        {
            Assert.IsFalse(DataUrl.TryParse(input, out DataUrl _));
        }

        [TestMethod]
        public void GetFileTypeExtensionTest()
        {
            var dataUrl = DataUrl.Parse("data:,abc");
            Assert.AreEqual(".txt", dataUrl.GetFileTypeExtension());
        }
        

        [TestMethod]
        public void FromBytesTest2()
        {
            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType mime));

            byte[] bytes = new byte[] { 1, 2, 3 };
            string outText = DataUrl.CreateFromBytes(bytes, mime);

            Assert.IsNotNull(outText);

            Assert.IsTrue(DataUrl.TryParse(outText, out DataUrl dataUrl));

            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));

            CollectionAssert.AreEqual(bytes, outBytes);
        }

        [TestMethod]
        public void FromBytesTest3()
        {
            string outText = DataUrl.CreateFromBytes(null, MimeType.Empty);

            Assert.IsNotNull(outText);

            Assert.IsTrue(DataUrl.TryParse(outText, out DataUrl dataUrl));

            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));

            CollectionAssert.AreEqual(Array.Empty<byte>(), outBytes);
        }


        [TestMethod]
        public void FromFileTest1()
        {
            string path = TestFiles.FolkerPng;
            string url = DataUrl.CreateFromFile(path);
            Assert.IsNotNull(url);

            Assert.IsTrue(DataUrl.TryParse(url, out DataUrl dataUrl));

            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));

            CollectionAssert.AreEqual(outBytes, File.ReadAllBytes(path));
        }

        [TestMethod]
        public void FromFileTest2()
        {
            string path = TestFiles.EmptyTextFile;
            string url = DataUrl.CreateFromFile(path);
            Assert.IsNotNull(url);
            Assert.IsTrue(DataUrl.TryParse(url, out DataUrl dataUrl));
            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));
            CollectionAssert.AreEqual(outBytes, File.ReadAllBytes(path));
        }

        [TestMethod]
        public void FromTextOnNull()
        {
            string urlString = DataUrl.CreateFromText(null);
            Assert.IsNotNull(urlString);
            Assert.IsTrue(DataUrl.TryParse(urlString, out DataUrl dataUrl));
            Assert.IsTrue(dataUrl.TryGetEmbeddedText(out string? output));
            Assert.AreEqual(string.Empty, output);
        }

        [TestMethod]
        public void FromTextOnStringEmpty()
        {
            string urlString = DataUrl.CreateFromText("");
            Assert.IsTrue(DataUrl.TryParse(urlString, out DataUrl dataUrl));
            Assert.IsTrue(dataUrl.TryGetEmbeddedText(out string? output));
            Assert.AreEqual(string.Empty, output);
        }

        [TestMethod()]
        public void FromTextTest1()
        {
            const string TEXT = "In Märchenbüchern herumstöbern.";

            string dataUrl1 = DataUrl.CreateFromText(Uri.EscapeDataString(TEXT));

            Assert.IsTrue(DataUrl.TryParse(dataUrl1, out DataUrl dataUrl2));

            Assert.AreEqual(dataUrl2.MimeType.TopLevelMediaType.ToString(), "text");
            Assert.AreEqual(dataUrl2.MimeType.SubType.ToString(), "plain");

            Assert.AreEqual(0, dataUrl2.MimeType.Parameters.Count());

            Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outText));
            Assert.AreEqual(TEXT, outText);
        }



        [TestMethod()]
        public void FromTextTest2()
        {
            const string TEXT = "In Märchenbüchern herumstöbern.";

            string dataUrl1 = DataUrl.CreateFromText(TEXT);

            Assert.IsTrue(DataUrl.TryParse(dataUrl1, out DataUrl dataUrl2));
            Assert.AreEqual(dataUrl2.MimeType.TopLevelMediaType.ToString(), "text");
            Assert.AreEqual(dataUrl2.MimeType.SubType.ToString(), "plain");
            Assert.AreEqual(0, dataUrl2.MimeType.Parameters.Count());
            Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outText));
            Assert.AreEqual(TEXT, outText);
        }

        [TestMethod]
        public void FromTextTest3()
        {
            string text = "http://www.fölkerchen.de";
            //string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

            string outText = DataUrl.CreateFromText(text);

            Assert.IsNotNull(outText);
            Assert.IsTrue(DataUrl.TryParse(outText, out DataUrl dataUrl));
            Assert.IsTrue(dataUrl.TryGetEmbeddedText(out string? output));
            Assert.AreEqual(text, output);
        }
    }
}