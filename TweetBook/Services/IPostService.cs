using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetBook.Domain;

namespace TweetBook.Services
{
    public interface IPostService
    {
        //for without paginated result
        //Task<List<Post>> GetPostsAsync();

        //For paginated result
        Task<List<Post>> GetPostsAsync(GetAllPostsFilter filter = null, PaginationFilter paginationFilter = null);

        Task<Post> GetPostByIdAsync(Guid postId);

        Task<bool> CreatePostAsync(Post post);

        Task<bool> UpdatePostAsync(Post postToUpdate);

        Task<bool> DeletePostAsync(Guid postId);

        Task<bool> UserOwnsPostAsync(Guid postId, string userId);

        Task<List<Tag>> GetAllTagsAsync();
        
        Task<Tag> GetTagByNameAsync(string tagName);

        Task<bool> CreateTagAsync(Tag tag);
        
        Task<bool> DeleteTagAsync(string tagName);
    }
}
