using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class ByteConverter : ITypeConverter<byte>
    {
        public byte Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(byte);

            return stream[position];
        }

        public byte[] Serialize(byte obj)
        {
            return new[] { obj };
        }
    }
}
