﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.Uris;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace FolkerKinzel.Uris.Tests
{
    [TestClass()]
    public class MimeTypeTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseTest1()
            => _ = MimeType.Parse(null!);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTest2() => _ = MimeType.Parse(" \t \r\n");


        [TestMethod()]
        public void ToStringTest1()
        {
            string result = new MimeType().ToString();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod()]
        public void ToStringTest2()
        {
            const string input = "text/plain";
            Assert.IsTrue(MimeType.TryParse(input.AsMemory(), out MimeType media));
            string result = media.ToString();

            Assert.IsNotNull(result);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ToStringTest3()
        {
            Assert.IsTrue(MimeType.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1".AsMemory(), out MimeType inetMedia));

            Assert.AreEqual("text/plain;charset=iso-8859-1", inetMedia.ToString());
        }

        [DataTestMethod]
        [DataRow("text/plain; charset=iso-8859-1", true, 1)]
        [DataRow("text / plain; charset=iso-8859-1;;", true, 1)]
        [DataRow("text / plain; charset=iso-8859-1;second=;", true, 1)]
        [DataRow("text / plain; charset=iso-8859-1;second=\"Second ; Value\"", true, 2)]
        public void TryParseTest1(string input, bool expected, int parametersCount)
        {
            Assert.AreEqual(expected, MimeType.TryParse(input.AsMemory(), out MimeType mediaType));

            //int size = Marshal.SizeOf(ReadOnlyMemory<char>.Empty);
            //size = Marshal.SizeOf(mediaType);

            MimeTypeParameter[]? arr = mediaType.Parameters.ToArray();

            Assert.AreEqual(parametersCount, arr.Length);
        }

        [TestMethod]
        public void EqualsTest1()
        {
            string media1 = "text/plain; charset=iso-8859-1";
            string media2 = "TEXT/PLAIN; CHARSET=ISO-8859-1";

            var mediaType1 = MimeType.Parse(media1);
            var mediaType2 = MimeType.Parse(media2);

            Assert.IsTrue(mediaType1 == mediaType2);
            Assert.IsFalse(mediaType1 != mediaType2);

            Assert.AreEqual(mediaType1.GetHashCode(), mediaType2.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest2()
        {
            string media1 = "text/plain; charset=iso-8859-1;second=value";
            string media2 = "TEXT/PLAIN; CHARSET=ISO-8859-1;SECOND=VALUE";

            var mediaType1 = MimeType.Parse(media1);
            var mediaType2 = MimeType.Parse(media2);

            Assert.IsTrue(mediaType1 != mediaType2);
            Assert.IsFalse(mediaType1 == mediaType2);

            Assert.AreNotEqual(mediaType1.GetHashCode(), mediaType2.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest3()
        {
            Assert.IsTrue(MimeType.TryParse("text/plain; charset=us-ascii".AsMemory(), out MimeType media1));
            Assert.IsTrue(MimeType.TryParse("text/plain".AsMemory(), out MimeType media2));

            Assert.IsTrue(media1 == media2);
            Assert.IsFalse(media1 != media2);

            Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest4()
        {
            Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1".AsMemory(), out MimeType media1));
            Assert.IsTrue(MimeType.TryParse("text/plain".AsMemory(), out MimeType media2));

            Assert.IsTrue(media1 != media2);
            Assert.IsFalse(media1 == media2);

            Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest5()
        {
            Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1".AsMemory(), out MimeType media1));
            Assert.IsTrue(MimeType.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1".AsMemory(), out MimeType media2));

            Assert.IsTrue(media1 == media2);
            Assert.IsFalse(media1 != media2);

            Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest6()
        {
            Assert.IsTrue(MimeType.TryParse("text/plain; charset=iso-8859-1;other=value".AsMemory(), out MimeType media1));
            Assert.IsTrue(MimeType.TryParse("text/plain;charset=iso-8859-1;OTHER=VALUE".AsMemory(), out MimeType media2));

            Assert.IsTrue(media1 != media2);
            Assert.IsFalse(media1 == media2);

            Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
        }


        
    }
}