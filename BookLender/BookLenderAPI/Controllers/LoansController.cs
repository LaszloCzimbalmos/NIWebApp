using Microsoft.AspNetCore.Mvc;

namespace BookLenderAPI.Controllers
{
    public class LoansController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
