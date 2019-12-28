using Refit;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Sdk;

namespace TweekBook.Sdk.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cachedToken = string.Empty;

            var httpClientHandler = new HttpClientHandler
            {
                //This is to bypass SSL certification validation error
                //DANGER:Please remove this code for production
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri("https://localhost:44383")
            };

            var identityApi = RestService.For<IIdentityApi>(httpClient);
            //var identityApi = RestService.For<IIdentityApi>("https://localhost:5001");


            var registerResponse = await identityApi.RegisterAsync(new UserRegistrationRequest
            {
                Email = "sdkaccount@test.com",
                Password = "Test@123"
            });

            var loginResponse = await identityApi.LoginAsync(new UserLoginRequest
            {
                Email = "sdkaccount@test.com",
                Password = "Test@123"
            });

            cachedToken = loginResponse.Content.Token;

            var tweetbookApi = RestService.For<ITweetBookApi>("https://localhost:44383", new RefitSettings
            {
                AuthorizationHeaderValueGetter = () => Task.FromResult(cachedToken)
            });

            var allPosts = await tweetbookApi.GetAllPostsAsync();

            var createdPost = await tweetbookApi.CreatePostAsync(new CreatePostRequest
            {
                Name = "this is created by the SDK",
                Tags = new[] { "SDK Tag" }
            });

            var retrievedPost = await tweetbookApi.GetPostAsync(createdPost.Content.Id);

            var updatedPost = await tweetbookApi.UpdatePostAsync(createdPost.Content.Id, new UpdatePostRequest
            { 
                Name = "this post is updated by the SDK"
            });

            var deletedPost = await tweetbookApi.DeletePostAsync(createdPost.Content.Id);
        }

    }
}
