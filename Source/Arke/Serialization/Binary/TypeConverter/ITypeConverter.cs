using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary.TypeConverter
{
    interface ITypeConverter<T>
    {
        byte[] Serialize(T obj);

        T Deserialize(byte[] stream, int position, out int length);
    }
}
