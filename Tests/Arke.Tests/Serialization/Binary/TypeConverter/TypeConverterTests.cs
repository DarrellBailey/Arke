using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arke.Serialization.Binary.TypeConverter;
using Xunit;

namespace Arke.Tests.Serialization.Binary.TypeConverter
{
    public class TypeConverterTests
    {
        [Fact]
        public void CanConvertDecimal()
        {
            DecimalConverter converter = new DecimalConverter();

            int length;

            decimal value = decimal.MaxValue;

            byte[] serialized = converter.Serialize(value);

            decimal deserialized = converter.Deserialize(serialized, 0, out length);

            Assert.Equal(serialized.Length, length);

            Assert.Equal(value, deserialized);
        }

        [Fact]
        public void CanConvertByteArray()
        {
            ByteArrayConverter converter = new ByteArrayConverter();

            int length;

            byte[] bytes = { 1, 2, 3, 4 };

            byte[] serialized = converter.Serialize(bytes);

            byte[] deserialized = converter.Deserialize(serialized, 0, out length);

            Assert.Equal(serialized.Length, 8);

            Assert.Equal(serialized.Length, length);

            Assert.Equal(bytes[0], deserialized[0]);

            Assert.Equal(bytes[1], deserialized[1]);

            Assert.Equal(bytes[2], deserialized[2]);

            Assert.Equal(bytes[3], deserialized[3]);
        }

        [Fact]
        public void CanConvertString()
        {
            StringConverter converter = new StringConverter();

            int length;

            string value = "Hello World!";

            byte[] serialized = converter.Serialize(value);

            string deserialized = converter.Deserialize(serialized, 0, out length);

            Assert.Equal(serialized.Length, length);

            Assert.Equal(value, deserialized);
        }
    }
}
