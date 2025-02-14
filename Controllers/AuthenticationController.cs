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
        [HttpPost]
        public IActionResult Login(string Username, string Password, bool Remember)
        {
            if (Username == "admin" && Password == "password") // Dummy check
            {
                return Json(new { success = true, message = "Login successful!" });
            }
            return Json(new { success = false, message = "Invalid username or password." });
        }

    }
}
