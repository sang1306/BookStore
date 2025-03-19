using BookStore.Models;
using chat_application_demo.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            var user = UserSessionManager.GetUserInfo(HttpContext);

            // Check if user session exists
            if (user != null)
            {
                User u = _context.Users.Include(u => u.Books)
                    .FirstOrDefault(u => u.UserId == user.UserId);

                // Check if user was found in database
                if (u != null)
                {
                    // Make sure Books collection is not null
                    ViewBag.Books = u.Books ?? new List<BookStore.Models.Book>();
                }
                else
                {
                    ViewBag.Books = new List<BookStore.Models.Book>();
                }
            }
            else
            {
                ViewBag.Books = new List<BookStore.Models.Book>();
            }

            return View();
        }

        [HttpPost("{bookId}")]
        public ActionResult AddWishlist(int bookId)
        {

            var user = UserSessionManager.GetUserInfo(HttpContext);
            if (user == null)
            {
                return Unauthorized();
            }


            Book b = _context.Books.FirstOrDefault(b => b.BookId == bookId);
            if (b == null)
            {
                return NotFound();
            }
            User u = _context.Users.Include(u => u.Books).FirstOrDefault(u => u.UserId == user.UserId);

            if (u != null)
            {
                u.Books.Add(b);
                _context.SaveChanges();
            }

            return Ok();
        }
    }
}
