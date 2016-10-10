using Arke.Serialization.Binary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Arke.Tests.Serialization.Binary
{
    public class BinarySerializerTests
    {
        [Fact]
        public void CanSerialize()
        {
            SerializationTestType test = new SerializationTestType();

            test.Byte = byte.MaxValue;

            BinarySerializer serializer = new BinarySerializer();

            byte[] serialized = serializer.Serialize(test);

            SerializationTestType deserialized = serializer.Deserialize<SerializationTestType>(serialized);

            Assert.Equal(test.Byte, deserialized.Byte);
        }
    }
}
