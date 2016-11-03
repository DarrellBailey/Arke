using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arke.Api.ContentProcessors
{
    public interface IContentProcessor
    {
        Task<HttpContent> ProcessObject(Object obj);

        Task<T> ProcessContent<T>(HttpContent content);
    }
}
