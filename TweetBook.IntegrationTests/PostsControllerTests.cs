using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Contracts.V1;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Domain;
using Xunit;

namespace TweetBook.IntegrationTests
{
    public class PostsControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_withoutAnyPosts_ReturnsEmptyResponse()
        {
            //Arrange
            await AuthenticateAsync();

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.GetAll);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            //response.StatusCode.Should().Be(HttpStatusCode.NotAcceptable);
            (await response.Content.ReadAsAsync <List<Post>>()).Should().BeEmpty();
        }

        [Fact]
        public async Task Get_ReturnsPost_WhenPostExistsIntheDatabase()
        {
            //Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(new CreatePostRequest { Name = "Integration Test Post 1" });

            //Act
            var response = await TestClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Id.ToString()));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnPost = await response.Content.ReadAsAsync<Post>();
            returnPost.Id.Should().Be(returnPost.Id);
            returnPost.Name.Should().Be("Integration Test Post 1");
        }
    }
}
