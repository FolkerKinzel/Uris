using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FolkerKinzel.Uris.Extensions.Tests
{
    [TestClass]
    public class UriExtensionTests
    {
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
    }
}
