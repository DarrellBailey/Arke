using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class ByteArrayConverter : ITypeConverter<byte[]>
    {
        public byte[] Deserialize(byte[] stream, int position, out int length)
        {
            int arraySize = BitConverter.ToInt32(stream, position);

            length = arraySize + 4;

            position += 4;

            byte[] bytes = new byte[arraySize];

            Array.Copy(stream, position, bytes, 0, arraySize);

            return bytes;
        }

        public byte[] Serialize(byte[] obj)
        {
            //need to copy the array and encode the length in the bits

            List<byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(obj.Length));

            bytes.AddRange(obj);

            return bytes.ToArray(); 
        }
    }
}
