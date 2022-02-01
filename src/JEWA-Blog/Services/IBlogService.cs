using JEWA_Blog.Models;

namespace JEWA_Blog.Services
{
    public interface IBlogService
    {
        Task SavePost(Post post);
        Task DeletePost(Post post);

        Task<List<string>> GetCategories();

        Task<Post> GetPostById(string id);
        Task<Post> GetPostByTitle(string title);

        Task<List<Post>> GetPosts(int count,int skip=0,string category="");
    }
}
