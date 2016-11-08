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
                //get the given url
                string url = _uri.ToString();

                //get all the query parameters in a list
                List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();
                queryParams.AddRange(_queryParameters);
                queryParams.AddRange(_configuration.QueryParameters);

                //do replaces on any query parameters that have bracket identifiers
                KeyValuePair<string, string>[] queryParamsArray = queryParams.ToArray();
                foreach(KeyValuePair<string, string> param in queryParamsArray)
                {
                    if(url.Contains("{" + param.Key + "}"))
                    {
                        url = url.Replace("{" + param.Key + "}", param.Value);
                        queryParams.Remove(param);
                    }
                }

                //any remaining queryparams will be appended to the url using the standard ?/& syntax
                if (queryParams.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(_uri.Query))
                    {
                        url += "?";
                    }

                    for (int i = 0; i < queryParams.Count; i++)
                    {
                        url += queryParams[i].Key + "=" + queryParams[i].Value;

                        if (i < queryParams.Count - 1) url += "&";
                    }
                }

                //we now have a fully built url
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
