using Microsoft.AspNetCore.Mvc;

namespace BookLenderAPI.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
