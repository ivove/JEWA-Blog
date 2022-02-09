using JEWA_Blog.Models;
using System.Globalization;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace JEWA_Blog.Services
{
    public class FileBlogService : IBlogService
    {
        private List<Post> _posts = new();

        private const string POSTS = "posts";

        private const string MARKDOWN = "markdown";

        private readonly string _xmlFolder;

        private readonly string _markdownFolder;

        public FileBlogService(IWebHostEnvironment env)
        {
            if (env is null) { throw new ArgumentNullException(nameof(env)); }
            _xmlFolder = Path.Combine(env.WebRootPath,POSTS);
            _markdownFolder = Path.Combine(env.WebRootPath,POSTS,MARKDOWN);
            Initialize();
        }

        public void DeletePost(Post post)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetCategories()
        {
            return _posts.Where(p => p.IsPublished && p.PublicationDate <= DateTime.Now)
                .Select(post => post.Category.ToLowerInvariant())
                .Distinct();
        }

        public Post? GetPostById(string id)
        {
            return _posts.FirstOrDefault(p => p.PostId == id);
        }

        public Post? GetPostByTitle(string title)
        {
            return _posts.FirstOrDefault(p => p.Title.Equals(title,StringComparison.OrdinalIgnoreCase));
        }

        public int GetPostCount(string category="",bool published = true)
        {
            if (category == "") { return _posts.Where(p=> (!published || (p.IsPublished && (p.PublicationDate <= DateTime.Now)))).Count(); }
            return _posts.Where(p =>(p.Category==category) && (!published || (p.IsPublished && (p.PublicationDate <= DateTime.Now)))).Count();
        }

        public IEnumerable<Post> GetPosts(int count, int skip = 0, string category = "", bool published = true)
        {
            var posts = _posts.Where(p => ((category == "") || (category == p.Category)) && (!published || (p.IsPublished && (p.PublicationDate <= DateTime.Now))))
                .Skip(skip).Take(count);
            return posts;
        }

        public void SavePost(Post post)
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
            _posts = _posts.OrderByDescending(p=>p.PublicationDate).ToList();
        }

        private void ReadPosts()
        {
            if (!Directory.Exists(_xmlFolder))
            {
                Directory.CreateDirectory(_xmlFolder);
            }

            if (!Directory.Exists(_markdownFolder))
            {
                Directory.CreateDirectory(_markdownFolder);
            }

            foreach (var file in Directory.EnumerateFiles(_xmlFolder, "*.xml", SearchOption.TopDirectoryOnly))
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
                var markdownFile = Path.Combine(_markdownFolder, post.PostId+".md");
                post.Content = File.ReadAllText(markdownFile);
                LoadComments(post,doc);
                _posts.Add(post);
            }
        }

        private static string ReadXmlValue(XElement doc, XName name, string defaultValue = "")
        {
            if ( doc.Element(name) is null) { return defaultValue; }
            if (doc.Element(name)?.Value is null) { return defaultValue; };
            #pragma warning disable CS8602 // The checks above avoid null values.
            return doc.Element(name).Value;
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private static void LoadComments(Post post,XElement doc)
        {
            var comments = doc.Element("comments");

            if (comments is null)
            {
                return;
            }

            foreach (var node in comments.Elements("comment"))
            {
                var comment = new Comment
                {
                    CommentId = ReadXmlValue(node, "id"),
                    Author = ReadXmlValue(node, "author"),
                    Email = ReadXmlValue(node, "email"),
                    Content = ReadXmlValue(node, "content"),
                    PublicationTime = DateTime.Parse(ReadXmlValue(node, "publicationDate", "2000-01-01 00:00:00"),
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                };
                LoadReplies(comment,node);
                post.Comments.Add(comment);
            }
        }

        private static void LoadReplies(Comment comment,XElement doc)
        {
            var replies = doc.Element("replies");

            if (replies is null)
            {
                return;
            }

            foreach (var node in replies.Elements("comment"))
            {
                var reply = new Comment
                {
                    CommentId = ReadXmlValue(node, "id"),
                    Author = ReadXmlValue(node, "author"),
                    Email = ReadXmlValue(node, "email"),
                    Content = ReadXmlValue(node, "content"),
                    ReplyTo = ReadXmlValue(node, "replyTo"),
                    PublicationTime = DateTime.Parse(ReadXmlValue(node, "publicationDate", "2000-01-01 00:00:00"),
                        CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal),
                };
                LoadReplies(reply, node);
                comment.Replies.Add(reply);
            }
        }
    }
}
