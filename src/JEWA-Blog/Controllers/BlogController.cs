using JEWA_Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace JEWA_Blog.Controllers
{
    public class BlogController : Controller
    {
        private const int PAGESIZE = 2;
        private readonly IBlogService _blog;

        public BlogController(IBlogService blog)
        {
            _blog = blog;
        }
        [Route("/blog/{page:int?}/{category?}")]
        public IActionResult Index([FromRoute] int page = 0,[FromRoute]string category="")
        {
            var posts = _blog.GetPosts(PAGESIZE, page*PAGESIZE,category);

            var postCount = _blog.GetPostCount();
            if (page > 0) 
            { 
                ViewBag.ShowNewer = true;
                ViewBag.NewerPage = page-1;
            } else { ViewBag.ShowNewer = false; }
            if (((page*PAGESIZE)+posts.Count())<postCount) 
            { 
                ViewBag.ShowOlder = true; 
                ViewBag.OlderPage = page+1;
            } else { ViewBag.ShowOlder = false; }

            ViewBag.Categories = _blog.GetCategories();
            ViewBag.CurrentCategory = category;

            return View(posts);
        }

        public IActionResult Post(string id)
        {
            var post = _blog.GetPostById(id);
            if (post == null) { return NotFound(); }
            return View(post);
        }

        [Route("/blog/post/title/{title}")]
        public IActionResult PostPermanent([FromRoute]string title)
        {
            if (title == null) { return NotFound(); }
            var post = _blog.GetPostByTitle(title);
            if (post == null) { return NotFound(); }
            return View("Post",post);
        }
    }
}
