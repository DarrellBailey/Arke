using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class SByteConverter : ITypeConverter<sbyte>
    {
        public sbyte Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(sbyte);

            return Convert.ToSByte(stream[position]);
        }

        public byte[] Serialize(sbyte obj)
        {
            return new[] { Convert.ToByte(obj) };
        }
    }
}
