using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Binary
{
    public class BinarySerializer : IArkeSerializer<byte[]>
    {
        public T Deserialize<T>(byte[] serialized)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize(object deserialized)
        {
            throw new NotImplementedException();
        }
    }
}
