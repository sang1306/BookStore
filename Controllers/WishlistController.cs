using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using BookStore.Utils;

namespace BookStore.Controllers
{
    public class WishlistController : Controller
    {
        private readonly Prn222BookshopContext _context;

        public WishlistController(Prn222BookshopContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var userInfo = UserSessionManager.GetUserInfo(HttpContext);
            if (userInfo == null)
            {
                return RedirectToAction("Index", "Authentication");
            }

            int userId = userInfo.UserId;
            var user = _context.Users.Include(u => u.Books).FirstOrDefault(u => u.UserId == userId);

            return View(user);
        }



        [HttpPost]
        public IActionResult AddToWishlist(int bookId)
        {
            var userInfo = UserSessionManager.GetUserInfo(HttpContext);
            if (userInfo == null)
            {
                return Json(new { success = false, message = "Please login account before use", redirectUrl = "/Authentication/Index" });
            }

            int userId = userInfo.UserId;
            var user = _context.Users.Include(u => u.Books).FirstOrDefault(u => u.UserId == userId);
            var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);

            if (user != null && book != null && !user.Books.Contains(book))
            {
                user.Books.Add(book);
                _context.SaveChanges();
                return Json(new { success = true, added = true, redirectUrl = "/Wishlist/Index" });
            }

            return Json(new { success = false, message = "Error adding book to wishlist" });
        }


        [HttpPost]
        public IActionResult RemoveFromWishlist(int bookId)
        {
            var userInfo = UserSessionManager.GetUserInfo(HttpContext);
            if (userInfo == null)
            {
                return Json(new { success = false, message = "Please login account before use" });
            }

            int userId = userInfo.UserId;
            var user = _context.Users.Include(u => u.Books).FirstOrDefault(u => u.UserId == userId);
            var book = _context.Books.FirstOrDefault(b => b.BookId == bookId);

            if (user != null && book != null && user.Books.Contains(book))
            {
                user.Books.Remove(book);
                _context.SaveChanges();
                return Json(new { success = true, message = "Remove Complete!" }); 
            }

            return Json(new { success = false, message = "Error removing book from wishlist!" });
        }
    }
 }
