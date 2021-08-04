﻿using FolkerKinzel.MimeTypes;
using FolkerKinzel.Uris.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace FolkerKinzel.Uris.Intls.Tests
{
    [TestClass]
    public class DataUrlExtensionTests
    {
        //private const string DATA_URL_PROTOCOL = "data:";
        //private const string BASE64 = ";base64,";

        

        //[TestMethod]
        //public void AppendDataUrlProtocolTest()
        //{
        //    var sb = new StringBuilder();
        //    Assert.AreEqual(sb, sb.Append(DATA_URL_PROTOCOL));
        //    Assert.AreEqual(DATA_URL_PROTOCOL, sb.ToString());
        //}

        //[TestMethod]
        //public void AppendBase64Test()
        //{
        //    var sb = new StringBuilder();
        //    Assert.AreEqual(sb, sb.Append(BASE64));
        //    Assert.AreEqual(BASE64, sb.ToString());
        //}

        [TestMethod]
        public void AppendMediaTypeTest1()
        {
            Assert.IsTrue(MimeType.TryParse("text/plain", out MimeType media));

            var sb = new StringBuilder();

            Assert.AreEqual(sb, sb.AppendMediaType(media));

            Assert.AreEqual("", sb.ToString());
        }

        [TestMethod]
        public void AppendMediaTypeTest2()
        {
            Assert.IsTrue(MimeType.TryParse("text/plain;charset=iso-8859-1", out MimeType media));

            var sb = new StringBuilder();

            Assert.AreEqual(sb, sb.AppendMediaType(media));

            Assert.AreEqual(";charset=iso-8859-1", sb.ToString());
        }

        [TestMethod]
        public void AppendMediaTypeTest3()
        {
            string input = "text/html;charset=iso-8859-1";
            Assert.IsTrue(MimeType.TryParse(input, out MimeType media));

            var sb = new StringBuilder();

            Assert.AreEqual(sb, sb.AppendMediaType(media));

            Assert.AreEqual(input, sb.ToString());
        }
    }
}
