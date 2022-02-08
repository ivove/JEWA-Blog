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
            if (page > 0) { ViewBag.ShowPrevious = true; } else { ViewBag.ShowPrevious = false; }
            var postCount = _blog.GetPostCount();
            if ((page*PAGESIZE)<postCount) { ViewBag.ShowNext = true; } else { ViewBag.ShowNext = false; }
            ViewBag.Categories = _blog.GetCategories();
            return View(posts);
        }
    }
}
