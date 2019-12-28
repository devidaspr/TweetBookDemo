using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TweetBook.Contracts.V1.Requests;
using TweetBook.Contracts.V1.Responses;

namespace TweetBook.Sdk
{
    [Headers("Authorization: bearer")]
    public interface ITweetBookApi
    {
        [Get("/api/v1/posts")]
        Task<ApiResponse<PagedResponse<PostResponse>>> GetAllPostsAsync();

        [Get("/api/v1/posts/{postId}")]
        Task<ApiResponse<Response<PostResponse>>> GetPostAsync(Guid postId);

        [Post("/api/v1/posts")]
        Task<ApiResponse<Response<PostResponse>>> CreatePostAsync([Body] CreatePostRequest createPostRequest);

        [Put("/api/v1/posts/{postId}")]
        Task<ApiResponse<Response<PostResponse>>> UpdatePostAsync(Guid postId, [Body] UpdatePostRequest updatePostRequest);

        [Delete("/api/v1/posts/{postId}")]
        Task<ApiResponse<string>> DeletePostAsync(Guid postId);
    }
}
