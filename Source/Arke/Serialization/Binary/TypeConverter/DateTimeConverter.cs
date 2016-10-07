using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class DateTimeConverter : ITypeConverter<DateTime>
    {
        public DateTime Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(long);

            return new DateTime(BitConverter.ToInt64(stream, position));
        }

        public byte[] Serialize(DateTime obj)
        {
            return BitConverter.GetBytes(obj.Ticks);
        }
    }
}
