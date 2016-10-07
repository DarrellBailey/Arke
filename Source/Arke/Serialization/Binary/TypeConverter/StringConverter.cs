using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class StringConverter : ITypeConverter<string>
    {
        private ByteArrayConverter bytesConverter = new ByteArrayConverter();

        public string Deserialize(byte[] stream, int position, out int length)
        {
            byte[] bytes = bytesConverter.Deserialize(stream, position, out length);

            return Encoding.UTF8.GetString(bytes);
        }

        public byte[] Serialize(string obj)
        {
            return bytesConverter.Serialize(Encoding.UTF8.GetBytes(obj));
        }
    }
}
