using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arke.Api
{
    /// <summary>
    /// Entry point for using Arke Api
    /// </summary>
    public class ArkeApiClient
    {
        /// <summary>
        /// Run a GET request against the default url binding for the given type.
        /// </summary>
        /// <param name="queryParameters">Optional query parameters to append to the url.</param>
        /// <returns></returns>
        public async Task<T> Get<T>(params KeyValuePair<string, string>[] queryParameters)
        {
            string url;

            bool exists = ArkeApiConfiguration.Default.TypeBindings.TryGetValue(typeof(T), out url);

            if (!exists)
            {
                throw new ArkeException("No bound url exists for the given type in the default configuration");
            }

            return await Get<T>(url, ArkeApiConfiguration.Default, queryParameters);
        }

        /// <summary>
        /// Run a GET request against the given url.
        /// </summary>
        /// <param name="url">The url to run the GET request against.</param>
        /// <param name="queryParameters">Optional query parameters to append to the url.</param>
        /// <typeparam name="T">The type for which to run the request.</typeparam>
        /// <returns>A deserialized instance of the given type from the data returned in the request.</returns>
        public async Task<T> Get<T>(string url, params KeyValuePair<string, string>[] queryParameters)
        {
            return await Get<T>(url, ArkeApiConfiguration.Default, queryParameters);
        }

        public async Task<T> Get<T>(ArkeApiConfiguration configuration, params KeyValuePair<string, string>[] queryParameters)
        {
            string url;

            bool exists = configuration.TypeBindings.TryGetValue(typeof(T), out url);

            if (!exists)
            {
                throw new ArkeException("No bound url exists for the given type in the given configuration");
            }

            return await Get<T>(url, configuration, queryParameters);
        }

        public async Task<T> Get<T>(string url, ArkeApiConfiguration configuration, params KeyValuePair<string, string>[] queryParameters)
        {
            ArkeApiRequest request = new ArkeApiRequest(url, HttpMethod.Get, null, queryParameters, configuration);

            ArkeApiResult result = await request.SendRequest();

            return await result.GetResponseAsObject<T>();
        }

        public async Task<T> Post<T>(object postData, params KeyValuePair<string, string>[] queryParameters)
        {
            string url;

            bool exists = ArkeApiConfiguration.Default.TypeBindings.TryGetValue(typeof(T), out url);

            if (!exists)
            {
                throw new ArkeException("No bound url exists for the given type in the default configuration");
            }

            return await Post<T>(url, postData, ArkeApiConfiguration.Default, queryParameters);
        }

        public async Task<T> Post<T>(string url, object postData, params KeyValuePair<string, string>[] queryParameters)
        {
            return await Post<T>(url, postData, ArkeApiConfiguration.Default, queryParameters);
        }

        public async Task<T> Post<T>(object postData, ArkeApiConfiguration configuration, params KeyValuePair<string, string>[] queryParameters)
        {
            string url;

            bool exists = configuration.TypeBindings.TryGetValue(typeof(T), out url);

            if (!exists)
            {
                throw new ArkeException("No bound url exists for the given type in the given configuration");
            }

            return await Post<T>(url, postData, configuration, queryParameters);
        }

        public async Task<T> Post<T>(string url, object postData, ArkeApiConfiguration configuration, params KeyValuePair<string, string>[] queryParameters)
        {
            ArkeApiRequest request = new ArkeApiRequest(url, HttpMethod.Post, postData, queryParameters, configuration);

            ArkeApiResult result = await request.SendRequest();

            return await result.GetResponseAsObject<T>();
        }
    }
}
