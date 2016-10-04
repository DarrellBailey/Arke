using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization
{
    public interface IArkeSerializer<SerializationType>
    {
        SerializationType Serialize(object deserialized);

        T Deserialize<T>(SerializationType serialized);
    }
}
