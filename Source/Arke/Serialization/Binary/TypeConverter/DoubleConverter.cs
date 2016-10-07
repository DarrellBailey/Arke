using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    internal class DoubleConverter : ITypeConverter<double>
    {
        public double Deserialize(byte[] stream, int position, out int length)
        {
            length = sizeof(double);

            return BitConverter.ToDouble(stream, position);
        }

        public byte[] Serialize(double obj)
        {
            return BitConverter.GetBytes(obj);
        }
    }
}
