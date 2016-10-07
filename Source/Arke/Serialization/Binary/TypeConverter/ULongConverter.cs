using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class ULongConverter : ITypeConverter<ulong>
    {
        public ulong Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(ulong);

            return BitConverter.ToUInt64(stream, position);
        }

        public byte[] Serialize(ulong obj)
        {
            return BitConverter.GetBytes(obj);
        }
    }
}
