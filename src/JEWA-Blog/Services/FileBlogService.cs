using JEWA_Blog.Models;
using System.Globalization;
using System.Xml.Linq;

namespace JEWA_Blog.Services
{
    public class FileBlogService : IBlogService
    {
        private List<Post> posts = new List<Post>();

        private const string POSTS = "posts";

        private const string MARKDOWN = "markdown";

        private readonly string xmlFolder;

        private readonly string markdownFolder;

        public FileBlogService(IWebHostEnvironment env)
        {
            if (env is null) { throw new ArgumentNullException(nameof(env)); }
            xmlFolder = Path.Combine(env.WebRootPath,POSTS);
            markdownFolder = Path.Combine(env.WebRootPath,POSTS,MARKDOWN);
            Initialize();
        }

        public Task DeletePost(Post post)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<string> GetCategories()
        {
            throw new NotImplementedException();
        }

        public Task<Post?> GetPostById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Post?> GetPostByTitle(string title)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPostCount(bool published = true)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Post> GetPosts(int count, int skip = 0, string category = "", bool published = true)
        {
            throw new NotImplementedException();
        }

        public Task SavePost(Post post)
        {
            throw new NotImplementedException();
        }

        private void Initialize()
        {
            ReadPosts();
            SortPosts();
        }

        private void SortPosts()
        {
            posts = posts.OrderByDescending(p=>p.PublicationDate).ToList();
        }

        private void ReadPosts()
        {
            if (!Directory.Exists(xmlFolder))
            {
                Directory.CreateDirectory(xmlFolder);
            }

            if (!Directory.Exists(markdownFolder))
            {
                Directory.CreateDirectory(markdownFolder);
            }

            foreach (var file in Directory.EnumerateFiles(xmlFolder, "*.xml", SearchOption.TopDirectoryOnly))
            {
                var doc = XElement.Load(file);
                var post = new Post
                {
                    PostId = ReadXmlValue(doc,"postId"),
                    Title = ReadXmlValue(doc, "title"),
                    Excerpt = ReadXmlValue(doc, "excerpt"),
                    Category = ReadXmlValue(doc, "category"),
                    PublicationDate = DateTime.Parse(ReadXmlValue(doc, "publicationDate"), CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal),
                    IsPublished = bool.Parse(ReadXmlValue(doc, "isPublished", "true")),
                };
                var markdownFile = Path.Combine(markdownFolder, post.PostId,".md");
                post.Content = File.ReadAllText(markdownFile);
                posts.Add(post);
            }
        }

        private static string ReadXmlValue(XElement doc, XName name, string defaultValue = "")
        {
            if ( doc.Element(name) is null) { return defaultValue; }
            if (doc.Element(name)?.Value is null) { return defaultValue; };
            return doc.Element(name).Value;
        }
    }
}
