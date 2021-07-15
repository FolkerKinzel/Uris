using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.Uris;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris.Tests
{
    [TestClass()]
    public class InternetMediaTypeTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseTest1()
            => _ = InternetMediaType.Parse(null!);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTest2() => _ = InternetMediaType.Parse(" \t \r\n");


        [TestMethod()]
        public void ToStringTest1()
        {
            var result = new InternetMediaType().ToString();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod()]
        public void ToStringTest2()
        {
            var input = "text/plain";
            Assert.IsTrue(InternetMediaType.TryParse(input.AsMemory(), out var media));
            var result = media.ToString();

            Assert.IsNotNull(result);
            Assert.AreEqual(input, result);
        }

        [TestMethod]
        public void ToStringTest3()
        {
            Assert.IsTrue(InternetMediaType.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1".AsMemory(), out var inetMedia));

            Assert.AreEqual("text/plain;charset=iso-8859-1", inetMedia.ToString());
        }

        [DataTestMethod]
        [DataRow("text/plain; charset=iso-8859-1", true, 1)]
        [DataRow("text / plain; charset=iso-8859-1;;", true, 1)]
        [DataRow("text / plain; charset=iso-8859-1;second=;", true, 1)]
        [DataRow("text / plain; charset=iso-8859-1;second=\"Second ; Value\"", true, 2)]
        public void TryParseTest1(string input, bool expected, int parametersCount)
        {
            Assert.AreEqual(expected, InternetMediaType.TryParse(input.AsMemory(), out var mediaType));

            var arr = mediaType.Parameters.ToArray();

            Assert.AreEqual(parametersCount, arr.Length);
        }

        [TestMethod]
        public void EqualsTest1()
        {
            var media1 = "text/plain; charset=iso-8859-1";
            var media2 = "TEXT/PLAIN; CHARSET=ISO-8859-1";

            Assert.IsTrue(media1 == media2);
            Assert.IsFalse(media1 != media2);

            Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest2()
        {
            var media1 = "text/plain; charset=iso-8859-1;second=value";
            var media2 = "TEXT/PLAIN; CHARSET=ISO-8859-1;SECOND=VALUE";

            Assert.IsTrue(media1 != media2);
            Assert.IsFalse(media1 == media2);

            Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest3()
        {
            Assert.IsTrue(InternetMediaType.TryParse("text/plain; charset=us-ascii".AsMemory(), out var media1));
            Assert.IsTrue(InternetMediaType.TryParse("text/plain".AsMemory(), out var media2));

            Assert.IsTrue(media1 == media2);
            Assert.IsFalse(media1 != media2);

            Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest4()
        {
            Assert.IsTrue(InternetMediaType.TryParse("text/plain; charset=iso-8859-1".AsMemory(), out var media1));
            Assert.IsTrue(InternetMediaType.TryParse("text/plain".AsMemory(), out var media2));

            Assert.IsTrue(media1 != media2);
            Assert.IsFalse(media1 == media2);

            Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest5()
        {
            Assert.IsTrue(InternetMediaType.TryParse("text/plain; charset=iso-8859-1".AsMemory(), out var media1));
            Assert.IsTrue(InternetMediaType.TryParse("TEXT/PLAIN ; CHARSET=ISO-8859-1".AsMemory(), out var media2));

            Assert.IsTrue(media1 == media2);
            Assert.IsFalse(media1 != media2);

            Assert.AreEqual(media1.GetHashCode(), media2.GetHashCode());
        }

        [TestMethod]
        public void EqualsTest6()
        {
            Assert.IsTrue(InternetMediaType.TryParse("text/plain; charset=iso-8859-1;other=value".AsMemory(), out var media1));
            Assert.IsTrue(InternetMediaType.TryParse("text/plain;charset=iso-8859-1;OTHER=VALUE".AsMemory(), out var media2));

            Assert.IsTrue(media1 != media2);
            Assert.IsFalse(media1 == media2);

            Assert.AreNotEqual(media1.GetHashCode(), media2.GetHashCode());
        }


        
    }
}