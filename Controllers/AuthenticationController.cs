using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [Route("Auth")]
    public class AuthenticationController : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }
    }
}
