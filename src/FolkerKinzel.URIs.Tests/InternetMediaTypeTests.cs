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
        public void ToStringTest()
        {
            Assert.Fail();
        }
    }
}