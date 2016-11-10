using Arke.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Arke.Tests.Api
{
    public class ArkeApiClientTests
    {
        private readonly ArkeApiClient _client = new ArkeApiClient();

        //test api from https://jsonplaceholder.typicode.com/
        private readonly string ApiRoot = "https://jsonplaceholder.typicode.com/";
        private readonly string ApiPosts = "https://jsonplaceholder.typicode.com/posts/{id}/";

        //test api from nasa picture of the day
        private readonly string NasaApiRoot = "https://api.nasa.gov/planetary/apod";
        private readonly KeyValuePair<string, string> NasaApiKey = new KeyValuePair<string, string>("api_key", "DEMO_KEY");

        public ArkeApiClientTests()
        {
            ArkeApiConfiguration.Default.TypeBindings.Add(typeof(Post), ApiPosts);
        }

        [Fact]
        public async void CanGet()
        {
            Post[] post = await _client.Get<Post[]>();

            Assert.NotNull(post);
        }

        [Fact]
        public async void CanPost()
        {
            Post post = new Post()
            {
                body = "foo",
                title = "bar",
                userId = 1
            };

            post = await _client.Post<Post>(post);

            Assert.NotNull(post);

            Assert.NotEqual(post.id, default(int));
        }

        public class Post
        {
            public int userId { get; set; }

            public int id { get; set; }

            public string title { get; set; }

            public string body { get; set; }
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
