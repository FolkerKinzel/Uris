using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.URIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.URIs.Tests
{
    [TestClass()]
    public class InternetMediaTypeTests
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InternetMediaTypeTest1() 
            => _ = new InternetMediaType(null!);

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void InternetMediaTypeTest2() => _ = new InternetMediaType(" \t \r\n");


        [TestMethod()]
        public void ToStringTest()
        {
            Assert.Fail();
        }
    }
}