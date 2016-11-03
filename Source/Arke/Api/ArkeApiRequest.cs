using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arke.Api
{
    internal class ArkeApiRequest
    {
        private readonly string _url;

        private readonly HttpMethod _method;

        private readonly object _content;

        private readonly ArkeContentType _responseContentType;

        private readonly ArkeApiConfiguration _configuration;

        public ArkeApiRequest(string url, HttpMethod method, object content, ArkeApiConfiguration configuration)
        {
            _url = url;

            _method = method;

            _content = content;

            _configuration = configuration;
        }

        public async Task<ArkeApiResult> SendRequest()
        {
            using (HttpClient client = new HttpClient())
            {
                if (_method == HttpMethod.Get)
                {
                    return new ArkeApiResult(await client.GetAsync(_url), _configuration);
                }
                if (_method == HttpMethod.Post)
                {
                    return new ArkeApiResult(await client.PostAsync(_url, await _configuration.RequestContentProcessor.ProcessObject(_content)), _configuration);
                }
                if (_method == HttpMethod.Delete)
                {
                    return new ArkeApiResult(await client.DeleteAsync(_url), _configuration);
                }
                if (_method == HttpMethod.Put)
                {
                    return new ArkeApiResult(await client.PutAsync(_url, await _configuration.RequestContentProcessor.ProcessObject(_content)), _configuration);
                }
            }

            throw new ArkeException("Currently, only GET,POST,PUT,DELTE are supported in EasyApi.");
        }
    }
}
