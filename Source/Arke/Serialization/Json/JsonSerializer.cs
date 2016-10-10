using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Serialization.Json
{
    public class JsonSerializer : IArkeSerializer<string>
    {
        public T Deserialize<T>(string serialized)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(serialized);
        }

        public string Serialize(object deserialized)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(deserialized);
        }
    }
}
