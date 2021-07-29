using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.Uris.Extensions.Tests
{
    [TestClass]
    public class StringExtensionTests
    {
        private const string DATA_URL_PROTOCOL = "data:";


        [DataTestMethod]
        [DataRow(DATA_URL_PROTOCOL, true)]
        [DataRow("data:bla", true)]
        [DataRow("DATA:bla", true)]
        [DataRow("dotu:bla", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public void IsDataUrlTest1(string? input, bool expected)
            => Assert.AreEqual(expected, input.IsDataUrl());

        
    }
}
