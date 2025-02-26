using BookStore.Models;
using BookStore.Services.Book;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookStore.Controllers
{
    [Route("/HomePage")]
    
    public class HomePageController : Controller
    {
        private readonly Prn222BookshopContext _context;
        private readonly ICategoryRepository _categoryRepository;
     
        public HomePageController(Prn222BookshopContext context, ICategoryRepository categoryRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            return RedirectToAction("ListBook");
        }

        //view sách
        [HttpGet]
        [Route("ListBook")]
        public async Task<IActionResult> ListBookAsync(string searchTitle, int? categoryId, string sortOrder, int page = 1)
        {
            int pageSize = 12; // Số sách trên mỗi trang

            var booksQuery = _context.Books.Include(b => b.Category).AsQueryable();

            // 🔍 Tìm kiếm theo tiêu chí (Title hoặc Author)
            if (!string.IsNullOrEmpty(searchTitle))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(searchTitle) || b.Author.Contains(searchTitle));
            }

            // 📂 Lọc theo danh mục
            if (categoryId.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.CategoryId == categoryId);
            }

            // 🔄 Sắp xếp theo tiêu chí
            ViewData["TitleSort"] = sortOrder == "title_asc" ? "title_desc" : "title_asc";
            ViewData["PriceSort"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";

            switch (sortOrder)
            {
                case "title_asc":
                    booksQuery = booksQuery.OrderBy(b => b.Title);
                    break;
                case "title_desc":
                    booksQuery = booksQuery.OrderByDescending(b => b.Title);
                    break;
                case "price_asc":
                    booksQuery = booksQuery.OrderBy(b => b.Price);
                    break;
                case "price_desc":
                    booksQuery = booksQuery.OrderByDescending(b => b.Price);
                    break;
            }

            // 📌 Tính tổng số trang trước khi phân trang
            int totalBooks = await booksQuery.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);

            // 📖 Pagination - Lấy danh sách sách theo trang
            var pagedBooks = await booksQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // 🏷 Gửi dữ liệu đến View
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SearchTitle = searchTitle;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(pagedBooks);
        }

        //view chi tiết sách
        [HttpGet]
        [Route("BookDetail/{id}")]
        public async Task<IActionResult> BookDetailAsync(int id)
        {
            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BookId == id);

            ViewBag.random4Book = await GetRandomBooksAsync(4);


            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
        // hàm lấy ngẫu nhiên 4 cuốn sách
        private async Task<List<Book>> GetRandomBooksAsync(int count = 4)
        {
            return await _context.Books
                .OrderBy(x => Guid.NewGuid())  // Sắp xếp ngẫu nhiên
                .Take(count)  // Chọn 4 cuốn sách
                .ToListAsync();
        }
    }
}
