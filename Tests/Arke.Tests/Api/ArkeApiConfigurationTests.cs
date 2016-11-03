using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arke.Api;
using Xunit;

namespace Arke.Tests.Api
{
    public class ArkeApiConfigurationTests
    {
        [Fact]
        public void CanClone()
        {
            ArkeApiConfiguration source = new ArkeApiConfiguration();

            ArkeApiConfiguration clone = source.Clone();

            Assert.Equal(source.RequestContentType, clone.RequestContentType);

            Assert.Equal(source.ResponseContentType, clone.ResponseContentType);

            Assert.Equal(source.TypeBindings, clone.TypeBindings);

            Assert.Equal(source.QueryParameters, clone.QueryParameters);

            Assert.Same(source.RequestContentProcessor, clone.RequestContentProcessor);

            Assert.Same(source.ResponseContentProcessor, clone.ResponseContentProcessor);
        }
    }
}
