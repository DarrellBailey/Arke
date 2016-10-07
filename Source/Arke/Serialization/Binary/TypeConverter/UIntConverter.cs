using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class UIntConverter : ITypeConverter<uint>
    {
        public uint Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(uint);

            return BitConverter.ToUInt32(stream, position);
        }

        public byte[] Serialize(uint obj)
        {
            return BitConverter.GetBytes(obj);
        }
    }
}
