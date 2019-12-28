using System;
using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using TweetBook.Contracts.V1;
using System.Threading.Tasks;

namespace TweetBook.IntegrationTests
{
    public class UnitTest1
    {
        private readonly HttpClient _client;

        public UnitTest1()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            _client = appFactory.CreateClient();
        }

        [Fact]
        public async Task Test1Async()
        {
            var response = await _client.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", "1"));
        }
    }
}
