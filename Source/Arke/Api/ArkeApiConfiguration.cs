using Arke.Api.ContentProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke.Api
{
    public class ArkeApiConfiguration
    {
        public static ArkeApiConfiguration Default { get; private set; }

        static ArkeApiConfiguration()
        {
            Default = new ArkeApiConfiguration();
        }

        public Dictionary<Type, string> TypeBindings { get; private set; }

        public ArkeContentType RequestContentType { get; private set; }

        public ArkeContentType ResponseContentType { get; private set; }

        public IContentProcessor RequestContentProcessor { get; private set; }

        public IContentProcessor ResponseContentProcessor { get; private set; }

        public ArkeApiConfiguration()
        {
            TypeBindings = new Dictionary<Type, string>();

            RequestContentType = ArkeContentType.Json;

            ResponseContentType = ArkeContentType.Json;

            RequestContentProcessor = new JsonContentProcessor();

            ResponseContentProcessor = new JsonContentProcessor();
        }
    }
}
