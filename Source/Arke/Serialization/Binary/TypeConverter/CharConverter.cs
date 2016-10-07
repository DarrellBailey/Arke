using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class CharConverter : ITypeConverter<char>
    {
        public char Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(char);

            return BitConverter.ToChar(stream, position);
        }

        public byte[] Serialize(char obj)
        {
            return BitConverter.GetBytes(obj);
        }
    }
}
