namespace FolkerKinzel.Uris.Intls.Tests
{
    [TestClass]
    public class Base64ParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseTest1() => _ = Base64Parser.Decode(null!);

        [DataTestMethod]
        [DataRow("ABCD")]
        [DataRow("ABC")]
        [DataRow("AB")]
        //[DataRow("A")]
        [DataRow("")]
        public void ParseTest2(string input) => Assert.IsNotNull(Base64Parser.Decode(input));
    }
}
