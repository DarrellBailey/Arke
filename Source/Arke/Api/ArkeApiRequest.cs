using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arke.Api
{
    internal class ArkeApiRequest
    {
        private Uri _uri;

        private readonly HttpMethod _method;

        private readonly object _content;

        private readonly ArkeApiConfiguration _configuration;

        private readonly KeyValuePair<string, string>[] _queryParameters;

        public ArkeApiRequest(string url, HttpMethod method, object content, KeyValuePair<string, string>[] queryParameters, ArkeApiConfiguration configuration)
        {
            _uri = new Uri(url);

            _method = method;

            _content = content;

            _configuration = configuration;

            _queryParameters = queryParameters;

            ProcessQueryVariables();
        }

        private void ProcessQueryVariables()
        {
            if (_queryParameters.Length > 0 || _configuration.QueryParameters.Count > 0)
            {
                List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();

                queryParams.AddRange(_queryParameters);

                queryParams.AddRange(_configuration.QueryParameters);

                string url = _uri.ToString();

                if (string.IsNullOrWhiteSpace(_uri.Query))
                {
                    url += "?";
                }

                for (int i = 0; i < queryParams.Count; i++)
                {
                    url += queryParams[i].Key + "=" + queryParams[i].Value;

                    if (i < queryParams.Count - 1) url += "&";
                }

                _uri = new Uri(url);
            }
        }

        public async Task<ArkeApiResult> SendRequest()
        {
            using (HttpClient client = new HttpClient())
            {
                if (_method == HttpMethod.Get)
                {
                    return new ArkeApiResult(await client.GetAsync(_uri.ToString()), _configuration);
                }
                if (_method == HttpMethod.Post)
                {
                    return new ArkeApiResult(await client.PostAsync(_uri.ToString(), await _configuration.RequestContentProcessor.ProcessObject(_content)), _configuration);
                }
                if (_method == HttpMethod.Delete)
                {
                    return new ArkeApiResult(await client.DeleteAsync(_uri.ToString()), _configuration);
                }
                if (_method == HttpMethod.Put)
                {
                    return new ArkeApiResult(await client.PutAsync(_uri.ToString(), await _configuration.RequestContentProcessor.ProcessObject(_content)), _configuration);
                }
            }

            throw new ArkeException("Currently, only GET,POST,PUT,DELTE are supported in Arke Api.");
        }
    }
}
