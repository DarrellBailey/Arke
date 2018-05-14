using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;

namespace Arke.Net.Tests
{
    [TestClass]
    public class ArkeMessage_Should
    {
        [TestMethod]
        public void CreateEmptyMessage()
        {
            ArkeMessage message = new ArkeMessage();

            Assert.AreEqual(message.Channel, 0);
            Assert.AreEqual(message.ContentType, ArkeContentType.None);
            Assert.AreEqual(message.Content.Length, 0);
            Assert.AreNotEqual(message.MessageId, Guid.Empty);
        }

        [TestMethod]
        public void CreateBytesMessage()
        {
            ArkeMessage message = new ArkeMessage(TestData.TestBytes);

            Assert.AreEqual(message.Channel, 0);
            Assert.AreEqual(message.ContentType, ArkeContentType.Bytes);
            Assert.IsTrue(Enumerable.SequenceEqual(message.GetContentAsBytes(), TestData.TestBytes));
            Assert.AreNotEqual(message.MessageId, Guid.Empty);
        }

        [TestMethod]
        public void CreateStringMessage()
        {
            ArkeMessage message = new ArkeMessage(TestData.TestString);

            Assert.AreEqual(message.Channel, 0);
            Assert.AreEqual(message.ContentType, ArkeContentType.String);
            Assert.AreEqual(message.GetContentAsString(), TestData.TestString);
            Assert.AreNotEqual(message.MessageId, Guid.Empty);
        }

        [TestMethod]
        public void CreateSerializedMessage()
        {
            ArkeMessage message = new ArkeMessage(TestData.TestObject);
            TestObject deserialized = (TestObject)message.GetContentAsObject();

            Assert.IsTrue(Enumerable.SequenceEqual(TestData.TestObject.TestBytes, deserialized.TestBytes));
            Assert.AreEqual(TestData.TestObject.TestDouble, deserialized.TestDouble);
            Assert.AreEqual(TestData.TestObject.TestInteger, deserialized.TestInteger);
            Assert.AreEqual(TestData.TestObject.TestString, deserialized.TestString);
        }


        [TestMethod]
        public void CreateBytesMessageWithChannel()
        {
            ArkeMessage message = new ArkeMessage(TestData.TestBytes, 100);

            Assert.AreEqual(message.Channel, 100);
            Assert.AreEqual(message.ContentType, ArkeContentType.Bytes);
            Assert.IsTrue(Enumerable.SequenceEqual(message.GetContentAsBytes(), TestData.TestBytes));
            Assert.AreNotEqual(message.MessageId, Guid.Empty);
        }

        [TestMethod]
        public void CreateStringMessageWithChannel()
        {
            ArkeMessage message = new ArkeMessage(TestData.TestString, 100);

            Assert.AreEqual(message.Channel, 100);
            Assert.AreEqual(message.ContentType, ArkeContentType.String);
            Assert.AreEqual(message.GetContentAsString(), TestData.TestString);
            Assert.AreNotEqual(message.MessageId, Guid.Empty);
        }

        [TestMethod]
        public void CreateSerializedMessageWithChannel()
        {
            ArkeMessage message = new ArkeMessage(TestData.TestObject, 100);
            TestObject deserialized = (TestObject)message.GetContentAsObject();

            Assert.AreEqual(message.Channel, 100);
            Assert.IsTrue(Enumerable.SequenceEqual(TestData.TestObject.TestBytes, deserialized.TestBytes));
            Assert.AreEqual(TestData.TestObject.TestDouble, deserialized.TestDouble);
            Assert.AreEqual(TestData.TestObject.TestInteger, deserialized.TestInteger);
            Assert.AreEqual(TestData.TestObject.TestString, deserialized.TestString);
        }


    }
}
