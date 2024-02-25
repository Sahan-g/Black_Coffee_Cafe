using Microsoft.AspNetCore.Mvc;

namespace Black_Coffee_Cafe.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
