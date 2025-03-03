using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BookStore.Controllers
{
    public class UserController : Controller
    {
        private readonly Prn222BookshopContext prn222BookshopContext;
        private readonly ILogger<UserController> _logger;

        public UserController(Prn222BookshopContext prn222BookshopContext, ILogger<UserController> logger)
        {
            this.prn222BookshopContext = prn222BookshopContext;
            _logger = logger;
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
			var user = prn222BookshopContext.Users.Find(id);
			if (user == null)
			{
				return NotFound();
			}
			return View(user);
		}

		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditProfile(User model)
		{
			

			var user = await prn222BookshopContext.Users.FindAsync(model.UserId);
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
				await prn222BookshopContext.SaveChangesAsync();
				_logger.LogInformation("Password updated successfully for UserId: {UserId}", user.UserId);
				return RedirectToAction("UserProfile", new { id = user.UserId });
			}
			catch (Exception ex)
			{
				_logger.LogError("Error updating password: {Message}", ex.Message);
				ModelState.AddModelError("", "Error saving data: " + ex.Message);
				return View(model);
			}


		}

		private string HashPassword(string password)
		{
			using (var sha256 = SHA256.Create())
			{
				var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
			}
		}

	}
}
