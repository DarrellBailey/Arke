using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class FloatConverter : ITypeConverter<float>
    {
        public float Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(float);

            return BitConverter.ToSingle(stream, position);
        }

        public byte[] Serialize(float obj)
        {
            return BitConverter.GetBytes(obj);
        }
    }
}
