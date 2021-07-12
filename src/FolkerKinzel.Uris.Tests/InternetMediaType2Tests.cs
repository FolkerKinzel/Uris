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
    public class InternetMediaType2Tests
    {
        [DataTestMethod]
        [DataRow("text/plain; charset=iso-8859-1", true)]
        public void TryParseTest1(string input, bool expected)
        {
            Assert.AreEqual(expected, InternetMediaType2.TryParse(input.AsMemory(), out InternetMediaType2 mediaType));
        }
    }
}