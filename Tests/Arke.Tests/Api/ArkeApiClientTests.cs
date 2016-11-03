﻿using Arke.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Arke.Tests.Api
{
    public class ArkeApiClientTests
    {
        private ArkeApiClient client = new ArkeApiClient(); 

        public ArkeApiClientTests()
        {
            ArkeApiConfiguration.Default.TypeBindings.Add(typeof(NasaPictureOfTheDay), "https://api.nasa.gov/planetary/apod");
        }

        [Fact]
        public async void CanGet()
        {
            NasaPictureOfTheDay pod = await client.Get<NasaPictureOfTheDay>(new KeyValuePair<string, string>("api_key", "DEMO_KEY"));

            Assert.NotNull(pod);
        }

        public class NasaPictureOfTheDay
        {
            public string copyright { get; set; }

            public DateTime date { get; set; }

            public string explanation { get; set; }

            public string hdurl { get; set; }

            public string media_type { get; set; }

            public string service_version { get; set; }

            public string title { get; set; }

            public string url { get; set; }
        }

    }
}