using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TweetBook.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;

namespace TweetBook.IntegrationTests
{
    public class IntegrationTest : IDisposable
    {
        protected readonly HttpClient TestClient;
        private readonly IServiceProvider _serviceProvider;
        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder => 
                {
                //Comment this part if want to test with real database
                //but you need to clean up the data that is getting generated during the tests
                //Otherwise you can work with this in memory database that will be created
                //every time you run the tests
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb");
                        });

                        //var serviceProvider = new ServiceCollection()
                        //        .AddEntityFrameworkInMemoryDatabase()
                        //        .BuildServiceProvider();
                        //services.RemoveAll(typeof(DataContext));
                        //services.AddDbContext<DataContext>(options =>
                        //{
                        //    options.UseInMemoryDatabase("TestDb")
                        //    .UseInternalServiceProvider(serviceProvider);
                        //});
                        //var sp = services.BuildServiceProvider();
                        //using (var scope = sp.CreateScope())
                        //{
                        //    var scopedServices = scope.ServiceProvider;
                        //    var db = scopedServices.GetRequiredService<DataContext>();
                        //    //var logger = scopedServices
                        //    //    .GetRequiredService<ILogger<WebApplicationFactory<Startup>>>();
                        //    db.Database.EnsureDeleted();
                        //    db.Database.EnsureCreated();
                        //}

                    });
                });

            _serviceProvider = appFactory.Services;
            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        protected async Task<PostResponse> CreatePostAsync(CreatePostRequest request)
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Posts.Create, request);

            return await response.Content.ReadAsAsync<PostResponse>();
        }

        private async Task<string> GetJwtAsync()
        {
            //var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Login, new UserRegistrationRequest
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = "test@int.com",
                Password = "Test@1234"
            });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();

            return registrationResponse.Token;
        }

        public void Dispose()
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            context.Database.EnsureDeleted();
        }
    }
}
