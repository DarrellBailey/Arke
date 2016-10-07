using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class LongConverter : ITypeConverter<long>
    {
        public long Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(long);

            return BitConverter.ToInt64(stream, position);
        }

        public byte[] Serialize(long obj)
        {
            return BitConverter.GetBytes(obj);
        }
    }
}
