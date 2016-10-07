using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class BoolConverter : ITypeConverter<bool>
    {
        public bool Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(bool);

            return BitConverter.ToBoolean(stream, position);
        }

        public byte[] Serialize(bool obj)
        {
            return BitConverter.GetBytes(obj);
        }
    }
}
