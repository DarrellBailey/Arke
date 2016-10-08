using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            PropertyInfo[] properties = GetObjectProperties(deserialized);

            List<byte> serialized = new List<byte>();

            for(int i = 0; i < properties.Length; i++)
            {
                byte[] bytes = GetPropertyBytes(deserialized, properties[i]);

                serialized.AddRange(bytes);
            }

            return serialized.ToArray();
        }

        private PropertyInfo[] GetObjectProperties(object obj)
        {
            return obj.GetType().GetProperties(BindingFlags.Public).Where(x => x.CanRead && x.CanWrite).OrderBy(x => x.Name).ToArray();
        }

        private byte[] GetPropertyBytes(object obj, PropertyInfo propertyInfo)
        {
            return new byte[0];
        }
    }
}
