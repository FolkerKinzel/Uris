using FolkerKinzel.Uris.Intls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace FolkerKinzel.Uris.Tests
{
    [TestClass]
    public class DataUrlInfoTests
    {
        private const string DEFAULT_ENCODING = "UrlEncoding";
        private const string DATA_PROTOCOL = "data:";

        [TestMethod]
        public void IsEmptyTest1() => Assert.IsTrue(DataUrlInfo.Empty.IsEmpty);

        [TestMethod]
        public void IsEmptyTest2()
        {
            _ = DataUrl.TryParse("data:,abc", out DataUrlInfo dataUrl);
            Assert.IsFalse(dataUrl.IsEmpty);
        }

        [TestMethod]
        public void TryParseTest1()
        {
            string text = "http://www.fölkerchen.de";


            string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

            Assert.IsTrue(DataUrl.TryParse(test, out DataUrlInfo dataUri));

            Assert.IsTrue(dataUri.TryGetEmbeddedText(out string? outText));
            Assert.AreEqual(text, outText);

            outText = DataUrl.FromText(text, "");

            Assert.IsNotNull(outText);

            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType? mime));

            outText = DataUrl.FromBytes(new byte[] { 1, 2, 3 }, mime);

            Assert.IsNotNull(outText);
        }

        [TestMethod]
        public void TryParseTest2()
        {
            string text = "http://www.fölkerchen.de";
            //string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

            string outText = DataUrl.FromText(text, "");

            Assert.IsNotNull(outText);

            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType? mime));

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

            Assert.IsTrue(DataUrl.TryParse(test, out DataUrlInfo dataUrl2));

            Assert.AreEqual(dataUrl2.Data.ToString(), "Text");
            Assert.AreEqual(dataUrl2.MimeType.ToString(), "text/plain;charset=UTF-8");

            Assert.AreEqual(dataUrl2.DataEncoding, DataEncoding.Url);
            Assert.AreEqual(MimeTypeInfo.Parse(dataUrl2.MimeType).Parameters().First().Value.ToString(), "UTF-8");


            Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outString));
            Assert.AreEqual("Text", outString);
        }

        [TestMethod]
        public void TryParseTest5()
        {
            const string url = "data:application/x-octet,A%42C";
            byte[] data = "ABC"u8.ToArray();

            Assert.IsTrue(DataUrl.TryParse(url, out DataUrlInfo dataUrl));
            Assert.AreEqual(DataEncoding.Url, dataUrl.DataEncoding);
            Assert.IsTrue(dataUrl.ContainsEmbeddedBytes);

            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? output));

            CollectionAssert.AreEqual(data, output);
        }

        [DataTestMethod]
        [DataRow("data:abc")]
        //[DataRow("data:,a bc")]
        public void TryParseTest7(string input) => Assert.IsFalse(DataUrl.TryParse(input, out _));

        [TestMethod]
        public void TryParseTest8()
        {
            const string data = "Märchenbücher";
            const string isoEncoding = "iso-8859-1";

            string s = $"data:;charset={isoEncoding};base64,{Convert.ToBase64String(TextEncodingConverter.GetEncoding(isoEncoding).GetBytes(data))}";

            Assert.IsTrue(DataUrl.TryParse(s, out DataUrlInfo _));
        }

        [TestMethod]
        public void TryParseTest9()
        {
            var sb = new StringBuilder(256 * 3);

            for (int i = 0; i < 256; i++)
            {
                _ = sb.Append('%').Append(i.ToString("x2"));
            }

            Assert.IsTrue(DataUrl.TryParse($"data:application/octet-stream,{sb}", out DataUrlInfo dataUrl));
            Assert.IsTrue(dataUrl.ContainsEmbeddedBytes);
            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? bytes));
            Assert.AreEqual(256, bytes!.Length);

            for (int i = 0; i < bytes!.Length; i++)
            {
                Assert.AreEqual(i, bytes[i]);
            }
        }

        [TestMethod]
        public void TryParseTest10()
        {
            const string text = "This is long Ascii text.";
            string urlString = DataUrl.FromText(text);

            Assert.IsTrue(DataUrl.TryParse(urlString, out DataUrlInfo dataUrl));
            Assert.AreEqual(DataEncoding.Url, dataUrl.DataEncoding);
            Assert.IsTrue(dataUrl.ContainsEmbeddedText);
            Assert.IsFalse(dataUrl.ContainsEmbeddedBytes);
            Assert.IsTrue(dataUrl.TryGetEmbeddedText(out string? outText));
            Assert.AreEqual(text, outText);
        }

        [TestMethod]
        public void TryParseTest11()
        {
            const string input = "data:blabla,abc";
            Assert.IsTrue(DataUrl.TryParse(input, out DataUrlInfo info));
            Assert.AreEqual("abc", info.Data.ToString());
            Assert.AreEqual(".bin", info.GetFileTypeExtension());   
        }

        [TestMethod]
        public void TryParseTest12()
        {
            byte[] data = new byte[] { 1, 2, 3 };
            string url = DataUrl.FromBytes(data, MimeType.Parse("application/x-stuff; key=\";bla,blabla\""));
            Assert.IsTrue(DataUrl.TryParse(url, out DataUrlInfo dataUrl));
            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? parsed));
            CollectionAssert.AreEqual(data, parsed);
        }

        [TestMethod]
        public void TryParseTest13()
        {
            ReadOnlyMemory<char> mem = "data:application/octet-stream;base64,ABCD".AsMemory();
            Assert.IsTrue(DataUrl.TryParse(mem, out DataUrlInfo info));
            Assert.IsFalse(info.IsEmpty);
        }

        [TestMethod]
        public void TryParseTest14()
        {
            ReadOnlyMemory<char> mem = "blabla".AsMemory();
            Assert.IsFalse(DataUrl.TryParse(mem, out _));
        }

        [TestMethod]
        public void TryParseTest15()
        {
            const string mem = "blabla";
            Assert.IsFalse(DataUrl.TryParse(mem, out _));
        }

        [TestMethod]
        public void GetFileTypeExtensionTest()
        {
            Assert.IsTrue(DataUrl.TryParse("data:,abc", out DataUrlInfo dataUrl));
            Assert.AreEqual(".txt", dataUrl.GetFileTypeExtension());
        }


        [TestMethod]
        public void TryGetEmbeddedTextTest1()
        {
            Assert.IsTrue(DataUrl.TryParse("data:;base64,A", out DataUrlInfo info));
            Assert.IsFalse(info.TryGetEmbeddedText(out _));
        }

        [TestMethod]
        public void TryGetEmbeddedTextTest2()
        {
            Assert.IsTrue(DataUrl.TryParse("data:text/plain;charset=utf-8;base64,ABCD", out DataUrlInfo info));
            Assert.IsFalse(info.TryGetEmbeddedText(out _));
        }

        [TestMethod]
        public void TryGetEmbedddedTextTest3() => Assert.IsTrue(new DataUrlInfo().TryGetEmbeddedText(out _));


        [TestMethod]
        public void TryGetEmbedddedBytesTest1() => Assert.IsFalse(new DataUrlInfo().TryGetEmbeddedBytes(out _));

        [TestMethod]
        public void TryGetEmbeddedBytesTest2()
        {
            Assert.IsTrue(DataUrl.TryParse("data:application/octet-stream;base64,A", out DataUrlInfo info));
            Assert.IsFalse(info.TryGetEmbeddedBytes(out _));
        }


        [TestMethod]
        public void ToStringTest1()
        {
            const string input = "data:,This is unescaped ASCII text.";

            Assert.IsTrue(DataUrl.TryParse(input, out DataUrlInfo info));
            Assert.IsTrue(info.ContainsEmbeddedText);
            Assert.IsFalse(info.ContainsEmbeddedBytes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AppendToTest1() => _ = DataUrl.AppendEmbeddedText(null!, "", MimeType.Parse(MimeString.OctetStream));

        [TestMethod]
        public void AppendToTest2()
        {
            var stringBuilder = new StringBuilder();
            _ = DataUrl.AppendEmbeddedText(stringBuilder, null, MimeType.Parse(MimeString.OctetStream));
            Assert.AreNotEqual(0, stringBuilder.Length);
        }

        [TestMethod]
        public void AppendToTest3()
        {
            var sb = new StringBuilder();

            Assert.IsTrue(DataUrl.TryParse("data:application/octet-stream,%01%02%03", out DataUrlInfo info));
            Assert.IsTrue(info.TryGetEmbeddedBytes(out byte[]? embeddedBytes));
            DataUrl.AppendEmbeddedBytes(sb, embeddedBytes, MimeType.Parse(MimeString.OctetStream));
            Assert.AreNotEqual(0, sb.Length);
        }

        [TestMethod]
        public void AppendToTest4()
        {
            var sb = new StringBuilder();

            Assert.IsTrue(DataUrl.TryParse("data:application/octet-stream;base64,ABCD", out DataUrlInfo info));
            Assert.IsTrue(info.TryGetEmbeddedBytes(out byte[]? embeddedBytes));
            _ = DataUrl.AppendEmbeddedBytes(sb, embeddedBytes, MimeType.Parse(MimeString.OctetStream));
            Assert.AreNotEqual(0, sb.Length);
        }


        [TestMethod]
        public void EqualsTest1()
        {
            const string input = "Märchenbücher";
            string urlStr1 = DataUrl.FromText(input);

            const string encodingName = "iso-8859-1";
            Encoding encoding = TextEncodingConverter.GetEncoding(encodingName);

            byte[]? bytes = encoding.GetBytes(input);

            var mime = MimeType.Parse($"text/plain; charset={encodingName}");
            string urlStr2 = DataUrl.FromBytes(bytes, mime);

            Assert.IsTrue(DataUrl.TryParse(urlStr1, out DataUrlInfo dataUrl1));
            Assert.IsTrue(DataUrl.TryParse(urlStr2, out DataUrlInfo dataUrl2));

            Assert.IsTrue(dataUrl1 == dataUrl2);
            Assert.IsFalse(dataUrl1 != dataUrl2);
            Assert.AreEqual(dataUrl1.GetHashCode(), dataUrl2.GetHashCode());

            Assert.IsTrue(dataUrl1.Equals(dataUrl2));

            object? o1 = dataUrl1;
            object? o2 = dataUrl2;

            //Assert.IsTrue(dataUrl1 == o2);
            //Assert.IsFalse(o1 != o2);
            Assert.AreEqual(o1.GetHashCode(), o2.GetHashCode());

        }


        [TestMethod]
        public void EqualsTest2()
        {
            string input = "xyz";
            string urlStr1 = $"data:application/octet-stream,{input}";
            string urlStr2 = $"data:application/octet-stream;base64,{Convert.ToBase64String(Encoding.ASCII.GetBytes(input))}";

            Assert.IsTrue(DataUrl.TryParse(urlStr1, out DataUrlInfo dataUrl1));
            Assert.IsTrue(DataUrl.TryParse(urlStr2, out DataUrlInfo dataUrl2));

            Assert.IsTrue(dataUrl1 == dataUrl2);
        }


        [TestMethod]
        public void EqualsTest3() => Assert.IsFalse(new DataUrlInfo().Equals(""));


        [TestMethod]
        public void EqualsTest4()
        {
            Assert.IsTrue(DataUrl.TryParse("data:,ABCD", out DataUrlInfo info1));
            Assert.IsTrue(DataUrl.TryParse("data:application/octet-stream;base64,ABCD", out DataUrlInfo info2));

            Assert.IsFalse(info1.Equals(info2));
            Assert.IsFalse(info2.Equals(info1));
        }

        [TestMethod]
        public void EqualsTest5()
        {
            Assert.IsTrue(DataUrl.TryParse("data:,A", out DataUrlInfo info1));
            Assert.IsTrue(DataUrl.TryParse("data:;base64,A", out DataUrlInfo info2));

            Assert.IsFalse(info2.Equals(info1));
        }

        [TestMethod]
        public void EqualsTest6()
        {
            Assert.IsTrue(DataUrl.TryParse("data:application/octet-stream;base64,A", out DataUrlInfo info1));
            Assert.IsTrue(DataUrl.TryParse("data:application/octet-stream,%01%02%03", out DataUrlInfo info2));

            Assert.IsFalse(info1.Equals(info2));
            Assert.IsFalse(info2.Equals(info1));
        }

        [TestMethod]
        public void CloneTest1()
        {
            Assert.IsTrue(DataUrl.TryParse("data:,xyz", out DataUrlInfo dtInfo));
            ICloneable info = dtInfo;

            object dataUrl2 = info.Clone();

            Assert.IsTrue(((DataUrlInfo)info) == (dataUrl2 as DataUrlInfo?));

        }

        [TestMethod]
        public void CloneTest2()
        {
            var info = (DataUrlInfo)new DataUrlInfo().Clone();
            Assert.IsTrue(info.IsEmpty);
        }

        [TestMethod]
        public void GetHashCodeTest1()
        {
            Assert.IsTrue(DataUrl.TryParse("data:application/octet-stream;base64,ABCD", out DataUrlInfo info2));
            Assert.AreNotEqual(new DataUrlInfo().GetHashCode(), info2.GetHashCode());
        }


        [TestMethod]
        public void LargeFileTest1()
        {
            byte[] buf = new byte[1024*1024];
            new Random().NextBytes(buf);
            
            string url = DataUrl.FromBytes(buf, MimeType.Parse("application/octet-stream"));
            Assert.IsTrue(DataUrl.TryParse(url, out DataUrlInfo info));
            Assert.IsTrue(info.TryGetEmbeddedBytes(out _));
        }

        [TestMethod]
        public void LargeFileTest2()
        {
            const string chunk = "%01%02%03";
            StringBuilder sb = new StringBuilder(chunk.Length * 20100);

            for (int i = 0; i < 20000; i++)
            {
                sb.Append(chunk);
            }

            string url = "data:application/octet-stream," + sb.ToString();
            Assert.IsTrue(DataUrl.TryParse(url, out DataUrlInfo info));
            Assert.IsTrue(info.TryGetEmbeddedBytes(out _));
        }

        [TestMethod]
        public void LargeFileTest3()
        {
            const string chunk = "%01%02%03";
            StringBuilder sb = new StringBuilder(chunk.Length * 20100);

            for (int i = 0; i < 20000; i++)
            {
                sb.Append(chunk);
            }

            string url = "data:," + sb.ToString();
            Assert.IsTrue(DataUrl.TryParse(url, out DataUrlInfo info));
            Assert.IsTrue(info.TryGetEmbeddedText(out _));
        }

    }
}
