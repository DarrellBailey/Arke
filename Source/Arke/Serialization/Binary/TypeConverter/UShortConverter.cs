using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class UShortConverter : ITypeConverter<ushort>
    {
        public ushort Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(ushort);

            return BitConverter.ToUInt16(stream, position);
        }

        public byte[] Serialize(ushort obj)
        {
            return BitConverter.GetBytes(obj);
        }
    }
}
