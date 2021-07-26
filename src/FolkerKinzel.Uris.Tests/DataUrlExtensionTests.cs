﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace FolkerKinzel.Uris.Tests
{
    [TestClass]
    public class DataUrlExtensionTests
    {
        private const string DATA_URL_PROTOCOL = "data:";
        private const string BASE64 = ";base64,";

        [DataTestMethod]
        [DataRow(DATA_URL_PROTOCOL, true)]
        [DataRow("data:bla", true)]
        [DataRow("DATA:bla", true)]
        [DataRow("dotu:bla", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public void IsDataUrlTest1(string? input, bool expected)
            => Assert.AreEqual(expected, input.IsDataUrl());

        [DataTestMethod]
        [DataRow("data:,", true)]
        [DataRow("DATA:,bla", true)]
        [DataRow("dotu:,bla", false)]
        [DataRow("http://www.contoso.com/", false)]
        [DataRow(null, false)]
        public void IsDataUrlTest2(string? input, bool expected)
        {
            Uri? uri = input is null ? null : new Uri(input);
            Assert.AreEqual(expected, uri.IsDataUrl());
        }

        [TestMethod]
        public void AppendDataUrlProtocolTest()
        {
            var sb = new StringBuilder();
            Assert.AreEqual(sb, sb.Append(DATA_URL_PROTOCOL));
            Assert.AreEqual(DATA_URL_PROTOCOL, sb.ToString());
        }

        [TestMethod]
        public void AppendBase64Test()
        {
            var sb = new StringBuilder();
            Assert.AreEqual(sb, sb.Append(BASE64));
            Assert.AreEqual(BASE64, sb.ToString());
        }

        [TestMethod]
        public void AppendMediaTypeTest1()
        {
            Assert.IsTrue(MimeType.TryParse("text/plain".AsMemory(), out MimeType media));

            var sb = new StringBuilder();

            Assert.AreEqual(sb, sb.AppendMediaType(media));

            Assert.AreEqual("", sb.ToString());
        }

        [TestMethod]
        public void AppendMediaTypeTest2()
        {
            Assert.IsTrue(MimeType.TryParse("text/plain;charset=iso-8859-1".AsMemory(), out MimeType media));

            var sb = new StringBuilder();

            Assert.AreEqual(sb, sb.AppendMediaType(media));

            Assert.AreEqual(";charset=iso-8859-1", sb.ToString());
        }

        [TestMethod]
        public void AppendMediaTypeTest3()
        {
            var input = "text/html;charset=iso-8859-1";
            Assert.IsTrue(MimeType.TryParse(input.AsMemory(), out MimeType media));

            var sb = new StringBuilder();

            Assert.AreEqual(sb, sb.AppendMediaType(media));

            Assert.AreEqual(input, sb.ToString());
        }
    }
}
