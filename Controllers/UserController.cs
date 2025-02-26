using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

		public IActionResult EditProfile(int id)
		{
			var user = prn222BookshopContext.Users.FirstOrDefault(u => u.UserId == id);
			if (user == null)
			{
				return NotFound();
			}
			return View(user);
		}

		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult EditProfile(User model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = prn222BookshopContext.Users.Find(model.UserId);
			if (user == null)
			{
				return NotFound();
			}

			user.Username = model.Username;
			user.Email = model.Email;
			user.Address = model.Address;
			user.Status = model.Status;

            try
            {
                prn222BookshopContext.Users.Update(user);
                prn222BookshopContext.SaveChanges(); 
                return RedirectToAction("UserProfile", new { id = user.UserId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error saving data: " + ex.Message);
                return View(model);
            }

		}

	}
}
