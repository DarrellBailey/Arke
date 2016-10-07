using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class IntConverter : ITypeConverter<int>
    {
        public int Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(int);

            return BitConverter.ToInt32(stream, position);
        }

        public byte[] Serialize(int obj)
        {
            return BitConverter.GetBytes(obj);
        }
    }
}
