using BookStore.Dtos.Book;
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

        [HttpPost]
        public IActionResult Create(Review model)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            if (ModelState.IsValid)
            {
                model.CreateAt = DateTime.Now; // Gán ngày tạo
                _context.Reviews.Add(model);
                _context.SaveChanges();

                // QUAY LẠI TRANG CHI TIẾT SÁCH SAU KHI GỬI ĐÁNH GIÁ
                return RedirectToAction("Details", "Books", new { id = model.BookId });
            }

            // NẾU DỮ LIỆU KHÔNG HỢP LỆ, Ở LẠI TRANG CHI TIẾT SÁCH
            return RedirectToAction("Details", "Books", new { id = model.BookId });
        }

    }
}
