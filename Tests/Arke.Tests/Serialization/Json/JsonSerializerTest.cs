using Arke.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Arke.Tests.Serialization.Json
{
    public class JsonSerializerTest
    {
        [Fact]
        public void CanSerialize()
        {
            SerializationTestType test = new SerializationTestType();

            test.Byte = byte.MaxValue;

            JsonSerializer serializer = new JsonSerializer();

            string serialized = serializer.Serialize(test);

            SerializationTestType deserialized = serializer.Deserialize<SerializationTestType>(serialized);

            Assert.Equal(test.Byte, deserialized.Byte);
        }
    }
}
