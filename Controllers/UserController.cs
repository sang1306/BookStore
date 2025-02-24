using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class UserController : Controller
    {
        private readonly Prn222BookshopContext prn222BookshopContext;

        public UserController(Prn222BookshopContext prn222BookshopContext)
        {
            this.prn222BookshopContext = prn222BookshopContext;
        }

        public IActionResult UserProfile(int id)
        {
            var user = prn222BookshopContext.Users.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

    }
}
