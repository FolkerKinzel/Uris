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
            _ = DataUrlInfo.TryParse("data:,abc", out DataUrlInfo dataUrl);
            Assert.IsFalse(dataUrl.IsEmpty);
        }

        [TestMethod]
        public void TryParseTest1()
        {
            string text = "http://www.fölkerchen.de";


            string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

            Assert.IsTrue(DataUrlInfo.TryParse(test, out DataUrlInfo dataUri));

            Assert.IsTrue(dataUri.TryGetEmbeddedText(out string? outText));
            Assert.AreEqual(text, outText);

            outText = DataUrlBuilder.FromText(text);

            Assert.IsNotNull(outText);

            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType mime));

            outText = DataUrlBuilder.FromBytes(new byte[] { 1, 2, 3 }, mime);

            Assert.IsNotNull(outText);
        }

        [TestMethod]
        public void TryParseTest2()
        {
            string text = "http://www.fölkerchen.de";
            //string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

            string outText = DataUrlBuilder.FromText(text);

            Assert.IsNotNull(outText);

            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType mime));

            outText = DataUrlBuilder.FromBytes(new byte[] { 1, 2, 3 }, mime);

            Assert.IsNotNull(outText);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("http://wwww.folker-kinzel.de/index.htm")]
        public void TryParseTest3(string? value) => Assert.IsFalse(DataUrlInfo.TryParse(value, out _));


        [TestMethod()]
        public void TryParseTest4()
        {
            string test = "data:;charset=UTF-8,Text";

            Assert.IsTrue(DataUrlInfo.TryParse(test, out DataUrlInfo dataUrl2));

            Assert.AreEqual(dataUrl2.Data.ToString(), "Text");
            Assert.AreEqual(dataUrl2.MimeType.MediaType.ToString(), "text");
            Assert.AreEqual(dataUrl2.MimeType.SubType.ToString(), "plain");

            Assert.AreEqual(dataUrl2.MimeType.Parameters().First().Value.ToString(), "UTF-8");
            Assert.AreEqual(dataUrl2.DataEncoding, DataEncoding.Url);

            Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outString));
            Assert.AreEqual("Text", outString);
        }

        [TestMethod]
        public void TryParseTest5()
        {
            const string url = "data:application/x-octet,A%42C";
            byte[] data = new byte[] { 0x41, 0x42, 0x43 };

            Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo dataUrl));
            Assert.AreEqual(DataEncoding.Url, dataUrl.DataEncoding);
            Assert.IsTrue(dataUrl.ContainsEmbeddedBytes);

            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? output));

            CollectionAssert.AreEqual(data, output);
        }

        [DataTestMethod]
        [DataRow("data:abc")]
        //[DataRow("data:,a bc")]
        public void TryParseTest7(string input) => Assert.IsFalse(DataUrlInfo.TryParse(input, out _));

        [TestMethod]
        public void TryParseTest8()
        {
            const string data = "Märchenbücher";
            const string isoEncoding = "iso-8859-1";

            string s = $"data:;charset={isoEncoding};base64,{Convert.ToBase64String(TextEncodingConverter.GetEncoding(isoEncoding).GetBytes(data))}";

            Assert.IsTrue(DataUrlInfo.TryParse(s, out DataUrlInfo dataUrlText1));
        }

        [TestMethod]
        public void TryParseTest9()
        {
            var sb = new StringBuilder(256 * 3);

            for (int i = 0; i < 256; i++)
            {
                _ = sb.Append('%').Append(i.ToString("x2"));
            }

            Assert.IsTrue(DataUrlInfo.TryParse($"data:application/octet-stream,{sb}", out DataUrlInfo dataUrl));
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
            string urlString = DataUrlBuilder.FromText(text);

            Assert.IsTrue(DataUrlInfo.TryParse(urlString, out DataUrlInfo dataUrl));
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
            Assert.IsFalse(DataUrlInfo.TryParse(input, out DataUrlInfo _));
        }

        [TestMethod]
        public void TryParseTest12()
        {
            var data = new byte[] { 1, 2, 3 };
            string url = DataUrlBuilder.FromBytes(data, MimeType.Parse("application/x-stuff; key=\";bla,blabla\""));
            Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo dataUrl));
            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? parsed));
            CollectionAssert.AreEqual(data, parsed);
        }

        [TestMethod]
        public void ParseTest1()
        {
            ReadOnlyMemory<char> mem = "data:application/octet-stream;base64,ABCD".AsMemory();
            var info = DataUrlInfo.Parse(in mem);
            Assert.IsFalse(info.IsEmpty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTest2()
        {
            ReadOnlyMemory<char> mem = "blabla".AsMemory();
            _ = DataUrlInfo.Parse(in mem);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTest3()
        {
            const string mem = "blabla";
            _ = DataUrlInfo.Parse(mem);
        }

        [TestMethod]
        public void GetFileTypeExtensionTest()
        {
            Assert.IsTrue(DataUrlInfo.TryParse("data:,abc", out DataUrlInfo dataUrl));
            Assert.AreEqual(".txt", dataUrl.GetFileTypeExtension());
        }


        [TestMethod]
        public void TryGetEmbeddedTextTest1()
        {
            Assert.IsTrue(DataUrlInfo.TryParse("data:;base64,A", out DataUrlInfo info));
            Assert.IsFalse(info.TryGetEmbeddedText(out _));
        }

        [TestMethod]
        public void TryGetEmbeddedTextTest2()
        {
            Assert.IsTrue(DataUrlInfo.TryParse("data:text/plain;charset=utf-8;base64,ABCD", out DataUrlInfo info));
            Assert.IsFalse(info.TryGetEmbeddedText(out _));
        }

        [TestMethod]
        public void TryGetEmbedddedTextTest3()
        {
            Assert.IsTrue(new DataUrlInfo().TryGetEmbeddedText(out _));
        }


        [TestMethod]
        public void TryGetEmbedddedBytesTest1()
        {
            Assert.IsFalse(new DataUrlInfo().TryGetEmbeddedBytes(out _));
        }

        [TestMethod]
        public void TryGetEmbeddedBytesTest2()
        {
            Assert.IsTrue(DataUrlInfo.TryParse("data:application/octet-stream;base64,A", out DataUrlInfo info));
            Assert.IsFalse(info.TryGetEmbeddedBytes(out _));
        }


        [TestMethod]
        public void ToStringTest1()
        {
            const string input = "data:,This is unescaped ASCII text.";

            Assert.IsTrue(DataUrlInfo.TryParse(input, out DataUrlInfo info));
            Assert.IsTrue(info.ContainsEmbeddedText);
            Assert.IsFalse(info.ContainsEmbeddedBytes);

            string output = info.ToString();

            Assert.AreNotEqual(input, output);
            Assert.IsTrue(DataUrlInfo.TryParse(output, out DataUrlInfo dataUrl2));
            Assert.AreEqual(info, dataUrl2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AppendToTest1() => _ = new DataUrlInfo().AppendTo(null!);

        [TestMethod]
        public void AppendToTest2()
        {
            var stringBuilder = new StringBuilder();
            _ = new DataUrlInfo().AppendTo(stringBuilder);
            Assert.AreNotEqual(0, stringBuilder.Length);
        }

        [TestMethod]
        public void AppendToTest3()
        {
            var sb = new StringBuilder();

            var info = DataUrlInfo.Parse("data:application/octet-stream,%01%02%03");
            _ = info.AppendTo(sb);
            Assert.AreNotEqual(0, sb.Length);
        }

        [TestMethod]
        public void AppendToTest4()
        {
            var sb = new StringBuilder();

            var info = DataUrlInfo.Parse("data:application/octet-stream;base64,ABCD");
            _ = info.AppendTo(sb);
            Assert.AreNotEqual(0, sb.Length);
        }


        [TestMethod]
        public void EqualsTest1()
        {
            const string input = "Märchenbücher";
            string urlStr1 = DataUrlBuilder.FromText(input);

            const string encodingName = "iso-8859-1";
            Encoding encoding = TextEncodingConverter.GetEncoding(encodingName);

            byte[]? bytes = encoding.GetBytes(input);

            var mime = MimeType.Parse($"text/plain; charset={encodingName}");
            string urlStr2 = DataUrlBuilder.FromBytes(bytes, in mime);

            Assert.IsTrue(DataUrlInfo.TryParse(urlStr1, out DataUrlInfo dataUrl1));
            Assert.IsTrue(DataUrlInfo.TryParse(urlStr2, out DataUrlInfo dataUrl2));

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

            Assert.IsTrue(DataUrlInfo.TryParse(urlStr1, out DataUrlInfo dataUrl1));
            Assert.IsTrue(DataUrlInfo.TryParse(urlStr2, out DataUrlInfo dataUrl2));

            Assert.IsTrue(dataUrl1 == dataUrl2);
        }


        [TestMethod]
        public void EqualsTest3() => Assert.IsFalse(new DataUrlInfo().Equals(""));


        [TestMethod]
        public void EqualsTest4()
        {
            Assert.IsTrue(DataUrlInfo.TryParse("data:,ABCD", out DataUrlInfo info1));
            Assert.IsTrue(DataUrlInfo.TryParse("data:application/octet-stream;base64,ABCD", out DataUrlInfo info2));

            Assert.IsFalse(info1.Equals(info2));
            Assert.IsFalse(info2.Equals(info1));
        }

        [TestMethod]
        public void EqualsTest5()
        {
            Assert.IsTrue(DataUrlInfo.TryParse("data:,A", out DataUrlInfo info1));
            Assert.IsTrue(DataUrlInfo.TryParse("data:;base64,A", out DataUrlInfo info2));

            Assert.IsFalse(info2.Equals(info1));
        }

        [TestMethod]
        public void EqualsTest6()
        {
            Assert.IsTrue(DataUrlInfo.TryParse("data:application/octet-stream;base64,A", out DataUrlInfo info1));
            Assert.IsTrue(DataUrlInfo.TryParse("data:application/octet-stream,%01%02%03", out DataUrlInfo info2));

            Assert.IsFalse(info1.Equals(info2));
            Assert.IsFalse(info2.Equals(info1));
        }

        [TestMethod]
        public void CloneTest1()
        {
            Assert.IsTrue(DataUrlInfo.TryParse("data:,xyz", out DataUrlInfo dtInfo));
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
            Assert.IsTrue(DataUrlInfo.TryParse("data:application/octet-stream;base64,ABCD", out DataUrlInfo info2));
            Assert.AreNotEqual(new DataUrlInfo().GetHashCode(), info2.GetHashCode());
        }


        [TestMethod]
        public void LargeFileTest1()
        {
            byte[] buf = new byte[1024*1024];
            new Random().NextBytes(buf);
            
            string url = DataUrlBuilder.FromBytes(buf, MimeType.Parse("application/octet-stream"));
            Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo info));
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
            Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo info));
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
            Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo info));
            Assert.IsTrue(info.TryGetEmbeddedText(out _));
        }

    }
}
