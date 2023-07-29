namespace FolkerKinzel.Uris.Tests;

[TestClass]
public class DataUrlTests
{
    // private const string DEFAULT_MIME_TYPE = "text/plain;charset=us-ascii";
    // private const string DEFAULT_ENCODING = "UrlEncoding";
    // private const string DATA_PROTOCOL = "data:";

    //[TestMethod]
    //public void SchemeDelimiterTest() => Assert.AreEqual(":", DataUrl.SchemeDelimiter);

    //        [TestMethod]
    //        public void IsEmptyTest1() => Assert.IsTrue(DataUrlInfo.Empty.IsEmpty);

    //        [TestMethod]
    //        public void IsEmptyTest2()
    //        {
    //            _ = DataUrlInfo.TryParse("data:,abc", out DataUrlInfo dataUrl);
    //            Assert.IsFalse(dataUrl.IsEmpty);
    //        }

    //        [TestMethod]
    //        public void TryParseTest1()
    //        {
    //            string text = "http://www.fölkerchen.de";


    //            string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

    //            Assert.IsTrue(DataUrlInfo.TryParse(test, out DataUrlInfo dataUri));

    //            Assert.IsTrue(dataUri.TryGetEmbeddedText(out string? outText));
    //            Assert.AreEqual(text, outText);

    //            outText = DataUrl.BuildFromEmbeddedText(text);

    //            Assert.IsNotNull(outText);

    //            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType mime));

    //            outText = DataUrl.BuildFromEmbeddedBytes(new byte[] { 1, 2, 3 }, mime);

    //            Assert.IsNotNull(outText);
    //        }

    //        [TestMethod]
    //        public void TryParseTest2()
    //        {
    //            string text = "http://www.fölkerchen.de";
    //            //string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

    //            string outText = DataUrl.BuildFromEmbeddedText(text);

    //            Assert.IsNotNull(outText);

    //            Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType mime));

    //            outText = DataUrl.BuildFromEmbeddedBytes(new byte[] { 1, 2, 3 }, mime);

    //            Assert.IsNotNull(outText);
    //        }

    //        [DataTestMethod]
    //        [DataRow(null)]
    //        [DataRow("")]
    //        [DataRow("http://wwww.folker-kinzel.de/index.htm")]
    //        public void TryParseTest3(string? value) => Assert.IsFalse(DataUrlInfo.TryParse(value, out _));


    //        [TestMethod()]
    //        public void TryParseTest4()
    //        {
    //            string test = "data:;charset=UTF-8,Text";

    //            Assert.IsTrue(DataUrlInfo.TryParse(test, out DataUrlInfo dataUrl2));

    //            Assert.AreEqual(dataUrl2.Data.ToString(), "Text");
    //            Assert.AreEqual(dataUrl2.MimeType.MediaType.ToString(), "text");
    //            Assert.AreEqual(dataUrl2.MimeType.SubType.ToString(), "plain");

    //            Assert.AreEqual(dataUrl2.MimeType.Parameters.First().Value.ToString(), "UTF-8");
    //            Assert.AreEqual(dataUrl2.Encoding, ContentEncoding.Url);

    //            Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outString));
    //            Assert.AreEqual("Text", outString);
    //        }

    //        [TestMethod]
    //        public void TryParseTest5()
    //        {
    //            const string url = "data:application/x-octet,A%42C";
    //            byte[] data = new byte[] { 0x41, 0x42, 0x43 };

    //            Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo dataUrl));
    //            Assert.AreEqual(ContentEncoding.Url, dataUrl.Encoding);
    //            Assert.IsTrue(dataUrl.ContainsEmbeddedBytes);

    //            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? output));

    //            CollectionAssert.AreEqual(data, output);
    //        }

    //        [DataTestMethod]
    //        [DataRow("data:abc")]
    //        //[DataRow("data:,a bc")]
    //        public void TryParseTest7(string input) => Assert.IsFalse(DataUrlInfo.TryParse(input, out DataUrlInfo _));

    //        [TestMethod]
    //        public void TryParseTest8()
    //        {
    //            const string data = "Märchenbücher";
    //            const string isoEncoding = "iso-8859-1";

    //#if NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
    //            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    //#endif
    //            string s = $"data:;charset={isoEncoding};base64,{Convert.ToBase64String(Encoding.GetEncoding(isoEncoding).GetBytes(data))}";

    //            DataUrlInfo dataUrlText1 = DataUrlInfo.Parse(s);
    //        }

    //        [TestMethod]
    //        public void TryParseTest9()
    //        {
    //            var sb = new StringBuilder(256 * 3);

    //            for (int i = 0; i < 256; i++)
    //            {
    //                _ = sb.Append('%').Append(i.ToString("x2"));
    //            }

    //            Assert.IsTrue(DataUrlInfo.TryParse($"data:application/octet-stream,{sb}", out DataUrlInfo dataUrl));
    //            Assert.IsTrue(dataUrl.ContainsEmbeddedBytes);
    //            Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? bytes));
    //            Assert.AreEqual(256, bytes!.Length);

    //            for (int i = 0; i < bytes!.Length; i++)
    //            {
    //                Assert.AreEqual(i, bytes[i]);
    //            }
    //        }

    //        [TestMethod]
    //        public void TryParseTest10()
    //        {
    //            string text = "This is long Ascii text.";
    //            string urlString = DataUrl.BuildFromEmbeddedText(text);

    //            Assert.IsTrue(DataUrlInfo.TryParse(urlString, out DataUrlInfo dataUrl));
    //            Assert.AreEqual(ContentEncoding.Url, dataUrl.Encoding);
    //            Assert.IsTrue(dataUrl.ContainsEmbeddedText);
    //            Assert.IsFalse(dataUrl.ContainsEmbeddedBytes);
    //            Assert.IsTrue(dataUrl.TryGetEmbeddedText(out string? outText));
    //            Assert.AreEqual(text, outText);
    //        }

    //        [TestMethod]
    //        public void GetFileTypeExtensionTest()
    //        {
    //            var dataUrl = DataUrlInfo.Parse("data:,abc");
    //            Assert.AreEqual(".txt", dataUrl.GetFileTypeExtension());
    //        }

    //[TestMethod]
    //public void MyTestMethod()
    //{
    //    string base64 = "Zg==";
    //    base64 = "Zg";
    //    base64 = HandleBase64WithoutPadding(base64);


    //    byte[] encr = Convert.FromBase64String(base64); 
    //    string decr = ASCIIEncoding.ASCII.GetString(encr);

    //    Assert.AreEqual("f", decr);
    //}



    [TestMethod]
    public void FromBytesTest2()
    {
        Assert.IsTrue(MimeType.TryParse("application/x-octet", out MimeType? mime));

        byte[] bytes = new byte[] { 1, 2, 3 };
        string outText = DataUrlBuilder.FromBytes(bytes, mime);

        Assert.IsNotNull(outText);

        Assert.IsTrue(DataUrlInfo.TryParse(outText, out DataUrlInfo dataUrl));

        Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));

        CollectionAssert.AreEqual(bytes, outBytes);
    }

    [TestMethod]
    public void FromBytesTest3()
    {
        string outText = DataUrlBuilder.FromBytes(null, MimeType.Parse("text/plain"));

        Assert.IsNotNull(outText);

        Assert.IsTrue(DataUrlInfo.TryParse(outText, out DataUrlInfo dataUrl));

        Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));

        CollectionAssert.AreEqual(Array.Empty<byte>(), outBytes);
    }


    [TestMethod]
    public void FromFileTest1()
    {
        string path = TestFiles.FolkerPng;
        string url = DataUrlBuilder.FromFile(path);
        Assert.IsNotNull(url);

        Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo dataUrl));

        Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));

        CollectionAssert.AreEqual(outBytes, File.ReadAllBytes(path));
    }

    [TestMethod]
    public void FromFileTest2()
    {
        string path = TestFiles.EmptyTextFile;
        string url = DataUrlBuilder.FromFile(path);
        Assert.IsNotNull(url);
        Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo dataUrl));
        Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));
        CollectionAssert.AreEqual(outBytes, File.ReadAllBytes(path));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void FromFileTest3() => _ = DataUrlBuilder.FromFile(null!);

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void FromFileTest4() => _ = DataUrlBuilder.FromFile("   ");

    [TestMethod]
    public void FromFileTest5()
    {
        string path = TestFiles.Utf8;
        string fileContent = File.ReadAllText(path);

        string url = DataUrlBuilder.FromFile(path);

        Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo dataUrl));
        Assert.IsTrue(dataUrl.TryGetEmbeddedText(out string? dataUrlText));

        Assert.AreEqual(fileContent, dataUrlText);
    }

    [TestMethod]
    public void FromFileTest6()
    {
        string path = TestFiles.Utf16LE;
        string fileContent = File.ReadAllText(path);

        string url = DataUrlBuilder.FromFile(path);

        Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo dataUrl));
        Assert.IsTrue(dataUrl.TryGetEmbeddedText(out string? dataUrlText));

        Assert.AreEqual(fileContent, dataUrlText);
    }

    //[TestMethod]
    //public void FromFileTest7()
    //{
    //    string path = TestFiles.FolkerPng;
    //    string url = DataUrlBuilder.FromFile(path, null, true);
    //    Assert.IsNotNull(url);

    //    Assert.IsTrue(DataUrlInfo.TryParse(url, out DataUrlInfo dataUrl));

    //    Assert.IsTrue(dataUrl.TryGetEmbeddedBytes(out byte[]? outBytes));

    //    CollectionAssert.AreEqual(outBytes, File.ReadAllBytes(path));
    //}


    [TestMethod]
    public void FromTextOnNull()
    {
        string urlString = DataUrlBuilder.FromText(null, ReadOnlyMemory<char>.Empty);
        Assert.IsNotNull(urlString);
        Assert.IsTrue(DataUrlInfo.TryParse(urlString, out DataUrlInfo dataUrl));
        Assert.IsTrue(dataUrl.TryGetEmbeddedText(out string? output));
        Assert.AreEqual(string.Empty, output);
    }

    [TestMethod]
    public void FromTextOnStringEmpty()
    {
        string urlString = DataUrlBuilder.FromText("", ReadOnlyMemory<char>.Empty);
        Assert.IsTrue(DataUrlInfo.TryParse(urlString, out DataUrlInfo dataUrl));
        Assert.IsTrue(dataUrl.TryGetEmbeddedText(out string? output));
        Assert.AreEqual(string.Empty, output);
    }

    [TestMethod()]
    public void FromTextTest1()
    {
        const string TEXT = "In Märchenbüchern herumstöbern.";

        string dataUrl1 = DataUrlBuilder.FromText(TEXT, ReadOnlyMemory<char>.Empty);

        Assert.IsTrue(DataUrlInfo.TryParse(dataUrl1, out DataUrlInfo dataUrl2));

        Assert.AreEqual(dataUrl2.MimeType.ToString(), "text/plain");

        Assert.AreEqual(1, MimeTypeInfo.Parse(dataUrl2.MimeType).Parameters().Count());

        Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outText));
        Assert.AreEqual(TEXT, outText);
    }



    //[TestMethod()]
    //public void FromTextTest2()
    //{
    //    const string TEXT = "In Märchenbüchern herumstöbern.";

    //    string dataUrl1 = DataUrl.FromText(TEXT);

    //    Assert.IsTrue(DataUrl.TryParse(dataUrl1, out DataUrl dataUrl2));
    //    Assert.AreEqual(dataUrl2.MimeType.TopLevelMediaType.ToString(), "text");
    //    Assert.AreEqual(dataUrl2.MimeType.SubType.ToString(), "plain");
    //    Assert.AreEqual(1, dataUrl2.MimeType.Parameters.Count());
    //    Assert.IsTrue(dataUrl2.TryGetEmbeddedText(out string? outText));
    //    Assert.AreEqual(TEXT, outText);
    //}

    [TestMethod]
    public void FromTextTest3()
    {
        string text = "http://www.fölkerchen.de";
        //string test = DATA_PROTOCOL + "text/plain;charset=utf-8" + ";" + DEFAULT_ENCODING + "," + Uri.EscapeDataString(text);

        string outText = DataUrlBuilder.FromText(text, ReadOnlyMemory<char>.Empty);

        Assert.IsNotNull(outText);
        Assert.IsTrue(DataUrlInfo.TryParse(outText, out DataUrlInfo dataUrl));
        Assert.IsTrue(dataUrl.TryGetEmbeddedText(out string? output));
        Assert.AreEqual(text, output);
    }


    //[TestMethod]
    //public void ToStringTest1()
    //{
    //    const string input = "data:,This is unescaped ASCII text.";

    //    Assert.IsTrue(DataUrlInfo.TryParse(input, out DataUrlInfo dataUrl));
    //    Assert.IsTrue(dataUrl.ContainsEmbeddedText);
    //    Assert.IsFalse(dataUrl.ContainsEmbeddedBytes);

    //    string output = dataUrl.ToString();

    //    Assert.AreNotEqual(input, output);
    //    Assert.IsTrue(DataUrlInfo.TryParse(output, out DataUrlInfo dataUrl2));
    //    Assert.AreEqual(dataUrl, dataUrl2);
    //}


    //[TestMethod]
    //public void EqualsTest1()
    //{
    //    const string input = "Märchenbücher";
    //    string urlStr1 = DataUrl.BuildFromEmbeddedText(input);

    //    const string encodingName = "iso-8859-1";
    //    Encoding encoding = TextEncodingConverter.GetEncoding(encodingName);

    //    byte[]? bytes = encoding.GetBytes(input);

    //    var mime = MimeType.Parse($"text/plain; charset={encodingName}");
    //    string urlStr2 = DataUrl.BuildFromEmbeddedBytes(bytes, in mime);

    //    var dataUrl1 = DataUrlInfo.Parse(urlStr1);
    //    var dataUrl2 = DataUrlInfo.Parse(urlStr2);

    //    Assert.IsTrue(dataUrl1 == dataUrl2);
    //    Assert.IsFalse(dataUrl1 != dataUrl2);
    //    Assert.AreEqual(dataUrl1.GetHashCode(), dataUrl2.GetHashCode());

    //    Assert.IsTrue(dataUrl1.Equals(dataUrl2));

    //    object? o1 = dataUrl1;
    //    object? o2 = dataUrl2;

    //    //Assert.IsTrue(dataUrl1 == o2);
    //    //Assert.IsFalse(o1 != o2);
    //    Assert.AreEqual(o1.GetHashCode(), o2.GetHashCode());

    //}


    //[TestMethod]
    //public void EqualsTest2()
    //{
    //    string input = "xyz";
    //    string urlStr1 = $"data:application/octet-stream,{input}";
    //    string urlStr2 = $"data:application/octet-stream;base64,{Convert.ToBase64String(Encoding.ASCII.GetBytes(input))}";

    //    var dataUrl1 = DataUrlInfo.Parse(urlStr1);
    //    var dataUrl2 = DataUrlInfo.Parse(urlStr2);

    //    Assert.IsTrue(dataUrl1 == dataUrl2);
    //}

    //[TestMethod]
    //public void CloneTest1()
    //{
    //    ICloneable dataUrl1 = DataUrlInfo.Parse("data:,xyz");

    //    object dataUrl2 = dataUrl1.Clone();

    //    Assert.IsTrue(((DataUrlInfo)dataUrl1) == (dataUrl2 as DataUrlInfo?));

    //}

}
