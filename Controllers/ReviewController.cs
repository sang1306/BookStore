using BookStore.Dtos.Book;
using BookStore.Filters;
using BookStore.Models;
using chat_application_demo.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class ReviewController : Controller
    {
        private readonly Prn222BookshopContext _context;

        public ReviewController(Prn222BookshopContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đánh giá của một cuốn sách
        public IActionResult Index(int bookId)
        {
            var reviews = _context.Reviews
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreateAt)
                .ToList();

            ViewBag.BookId = bookId;
            return View(reviews);
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost]
        public IActionResult Create(Review model)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            model.UserId = userSession.UserId;

            _context.Reviews.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Details", "Books", new { id = model.BookId });
        }

        [TypeFilter(typeof(AuthenticationFilter))]
        [HttpPost]
        public IActionResult Delete(int reviewId)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var review = _context.Reviews.FirstOrDefault(r => r.ReviewId == reviewId);
            if (review == null)
            {
                return NotFound();
            }

            // Chỉ cho phép người dùng xóa review của chính họ
            if (review.UserId != userSession.UserId)
            {
                return Forbid();
            }

            _context.Reviews.Remove(review);
            _context.SaveChanges();

            return RedirectToAction("Details", "Books", new { id = review.BookId });
        }

    }
}
