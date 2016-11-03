using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arke.Api
{
    internal class ArkeApiResult
    {
        private readonly HttpResponseMessage _response;

        private readonly ArkeApiConfiguration _configuration;

        public ArkeApiResult(HttpResponseMessage response,  ArkeApiConfiguration configuration)
        {
            _response = response;

            _configuration = configuration;
        }

        public async Task<T> GetResponseAsObject<T>()
        {
            return await _configuration.ResponseContentProcessor.ProcessContent<T>(_response.Content);
        }
    }
}
