using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arke.Api.ContentProcessors
{
    public class JsonContentProcessor : IContentProcessor
    {
        public Task<T> ProcessContent<T>(HttpContent content)
        {
            throw new NotImplementedException();
        }

        public Task<HttpContent> ProcessObject(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
