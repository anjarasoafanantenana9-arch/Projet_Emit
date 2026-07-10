using Microsoft.AspNetCore.Mvc;

namespace EMIT.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}