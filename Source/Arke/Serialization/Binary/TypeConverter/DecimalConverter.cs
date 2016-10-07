using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class DecimalConverter : ITypeConverter<decimal>
    {
        public decimal Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(decimal);

            int[] bits = new int[4];

            int bitIndex = 0;

            for(int i = position; i < position + 16; i += 4)
            {
                bits[bitIndex++] = BitConverter.ToInt32(stream, i);
            }

            return new decimal(bits);
        }

        public byte[] Serialize(decimal obj)
        {
            int[] ints = decimal.GetBits(obj);

            List<byte> bytes = new List<byte>();

            for(int i = 0; i < 4; i++)
            {
                bytes.AddRange(BitConverter.GetBytes(ints[i]));
            }

            return bytes.ToArray();
        }
    }
}
