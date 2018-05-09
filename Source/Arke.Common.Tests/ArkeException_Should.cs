using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arke.Common.Tests
{
    [TestClass]
    public class ArkeException_Should
    {
        [TestMethod]
        public void CreateWithMessage()
        {
            ArkeException exception = new ArkeException(TestData.TestString);
            Assert.AreEqual(exception.Message, TestData.TestString);
        }

        [TestMethod]
        public void CreateWithMessageAndInnerException()
        {
            ArkeException inner = new ArkeException(TestData.TestString);
            ArkeException exception = new ArkeException(TestData.TestString, inner);
            Assert.AreEqual(exception.Message, TestData.TestString);
            Assert.AreEqual(exception.InnerException, inner);
            Assert.AreEqual(exception.InnerException.Message, TestData.TestString);
        }
    }
}
