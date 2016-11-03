using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Arke.Api.ContentProcessors
{
    internal class JsonContentProcessor : IContentProcessor
    {
        public async Task<T> ProcessContent<T>(HttpContent content)
        {
            string json = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<HttpContent> ProcessObject(object obj)
        {
            string serialized = JsonConvert.SerializeObject(obj);

            byte[] bytes = Encoding.UTF8.GetBytes(serialized);

            MemoryStream stream = new MemoryStream(bytes);

            return new StreamContent(stream);
        }
    }
}
