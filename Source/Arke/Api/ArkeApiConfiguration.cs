using Arke.Api.ContentProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Api
{
    /// <summary>
    /// The configuration that Arke Api will use to build api requests and responses.
    /// </summary>
    public class ArkeApiConfiguration
    {
        /// <summary>
        /// The default Arke Api configuration.
        /// </summary>
        public static ArkeApiConfiguration Default { get; private set; }

        static ArkeApiConfiguration()
        {
            Default = new ArkeApiConfiguration();
        }

        private ArkeContentType _requestContentType;

        private ArkeContentType _responseContentType;

        /// <summary>
        /// A mapping between a type and a url that will service that type.
        /// </summary>
        public Dictionary<Type, string> TypeBindings { get; private set; }

        /// <summary>
        /// The processor that will be run to convert an object into the request.
        /// </summary>
        public IContentProcessor RequestContentProcessor { get; private set; }

        /// <summary>
        /// The processor that will be run to convert the response into an object.
        /// </summary>
        public IContentProcessor ResponseContentProcessor { get; private set; }

        /// <summary>
        /// Query parameters that will be appended to the url of all requests using this configuration.
        /// </summary>
        public List<KeyValuePair<string, string>> QueryParameters { get; private set; }

        /// <summary>
        /// The type of request arke api will try to create.
        /// </summary>
        public ArkeContentType RequestContentType
        {
            get
            {
                return _requestContentType;
            }

            set
            {
                switch (value)
                {
                    case ArkeContentType.Json:
                        _requestContentType = value;
                        RequestContentProcessor = new JsonContentProcessor();
                        break;
                    default:
                        throw new ArkeException("The chosen content type is not yet supported by Arke Api");
                }
            }
        }

        /// <summary>
        /// The type of response Arke Api should expect.
        /// </summary>
        public ArkeContentType ResponseContentType
        {
            get
            {
                return _responseContentType;
            }

            set
            {
                switch (value)
                {
                    case ArkeContentType.Json:
                        _responseContentType = value;
                        ResponseContentProcessor = new JsonContentProcessor();
                        break;
                    default:
                        throw new ArkeException("The chosen content type is not yet supported by Arke Api");
                }
            }
        }

        /// <summary>
        /// Create a new Arke Api Configuration
        /// </summary>
        public ArkeApiConfiguration()
        {
            TypeBindings = new Dictionary<Type, string>();

            RequestContentType = ArkeContentType.Json;

            ResponseContentType = ArkeContentType.Json;

            QueryParameters = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Clone this configuration.
        /// </summary>
        /// <returns>A clone of the configuration.</returns>
        public ArkeApiConfiguration Clone()
        {
            return new ArkeApiConfiguration
            {
                _requestContentType = _requestContentType,
                _responseContentType = _responseContentType,
                RequestContentProcessor = RequestContentProcessor,
                ResponseContentProcessor = ResponseContentProcessor,
                QueryParameters = QueryParameters.ToList(),
                TypeBindings = new Dictionary<Type, string>(TypeBindings)
            };
        }
    }
}
