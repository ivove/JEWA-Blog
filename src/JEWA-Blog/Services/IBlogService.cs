using JEWA_Blog.Models;

namespace JEWA_Blog.Services
{
    public interface IBlogService
    {
        Task SavePost(Post post);
        Task DeletePost(Post post);

        IAsyncEnumerable<string> GetCategories();

        Task<Post?> GetPostById(string id);
        Task<Post?> GetPostByTitle(string title);

        IAsyncEnumerable<Post> GetPosts(int count,int skip=0,string category="",bool published=true);

        Task<int> GetPostCount(bool published  = true);
    }
}
