using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class ShortConverter : ITypeConverter<short>
    {
        public short Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(short);

            return BitConverter.ToInt16(stream, position);
        }

        public byte[] Serialize(short obj)
        {
            return BitConverter.GetBytes(obj);
        }
    }
}
