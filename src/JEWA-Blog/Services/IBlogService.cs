using JEWA_Blog.Models;

namespace JEWA_Blog.Services
{
    public interface IBlogService
    {
        void SavePost(Post post);
        void DeletePost(Post post);

        IEnumerable<string> GetCategories();

        Post? GetPostById(string id);
        Post? GetPostByTitle(string title);

        IEnumerable<Post> GetPosts(int count,int skip=0,string category="",bool published=true);

        int GetPostCount(bool published  = true);
    }
}
