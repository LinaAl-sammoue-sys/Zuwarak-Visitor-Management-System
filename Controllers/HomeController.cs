using Microsoft.AspNetCore.Mvc;

namespace Zuwarak.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
