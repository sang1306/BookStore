using BookStore.Dtos.Book;
using BookStore.Models;
using chat_application_demo.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BookStore.Controllers
{
    [Route("/Books")]
    public class BooksController : Controller
    {
        private readonly Prn222BookshopContext _context;
        private readonly IWebHostEnvironment _env;
        private int CurrentPage
        {
            get => HttpContext.Session.GetInt32("CurrentPage") ?? 1;
            set => HttpContext.Session.SetInt32("CurrentPage", value);
        }

        public BooksController(Prn222BookshopContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Hiển thị danh sách thể loại
        [Route("viewCategories")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // Xử lý thêm thể loại sách 
        [HttpPost]
        [Route("AddCategory")]
        public async Task<IActionResult> AddCategory(Category category)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                ModelState.AddModelError("", "Tên thể loại không được để trống!");
                return RedirectToAction("Index"); // Quay lại danh sách
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Thêm thể loại thành công!";
            return RedirectToAction("Index");
        }

        // sửa thể loại
        [HttpPost]
        [Route("EditCategory")]
        public async Task<IActionResult> EditCategory(Category category)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
            if (existingCategory == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thể loại!";
                return RedirectToAction("Index");
            }

            existingCategory.CategoryName = category.CategoryName;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật thể loại thành công!";
            return RedirectToAction("Index");
        }

        //xóa thể loại
        [HttpPost]
        [Route("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Thể loại không tồn tại!";
                return RedirectToAction("Index");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa thể loại thành công!";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> AddBook(string searchTitle, int? categoryId, string sortOrder, int page = 1)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            int pageSize = 10; // Số sách trên mỗi trang

            var books = _context.Books.Include(b => b.Category).AsQueryable();

            // 🔍 Filter - Tìm kiếm theo tiêu chí
            if (!string.IsNullOrEmpty(searchTitle))
            {
                books = books.Where(b => b.Title.Contains(searchTitle) || b.Author.Contains(searchTitle));
            }

            if (categoryId.HasValue)
            {
                books = books.Where(b => b.CategoryId == categoryId);
            }

            // 🔄 Sort - Sắp xếp theo tiêu chí
            ViewData["TitleSort"] = sortOrder == "title_asc" ? "title_desc" : "title_asc";
            ViewData["PriceSort"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";

            switch (sortOrder)
            {
                case "title_asc":
                    books = books.OrderBy(b => b.Title);
                    break;
                case "title_desc":
                    books = books.OrderByDescending(b => b.Title);
                    break;
                case "price_asc":
                    books = books.OrderBy(b => b.Price);
                    break;
                case "price_desc":
                    books = books.OrderByDescending(b => b.Price);
                    break;
            }

            books = books.OrderByDescending(b => b.BookId);

            // 📖 Pagination - Phân trang
            var pagedBooks = await books.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)books.Count() / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.SearchTitle = searchTitle;
            ViewBag.CategoryId = categoryId;
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(pagedBooks);
        }

        //hàm lưu ảnh và trả về đường dẫn
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }

            // Tạo tên file duy nhất
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

            // Đường dẫn lưu file
            var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

            // Tạo thư mục nếu chưa tồn tại
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Lưu file vào thư mục
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/uploads/" + fileName;
        }

        // thêm mới book
        [HttpPost]
        [Route("addNewBook")]
        public async Task<IActionResult> AddNewBook(Book book, IFormFile ImageFile)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }

            try
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    book.Image = "/uploads/" + fileName;
                }

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Book added successfully!";

                // Truy vấn lại danh sách sách đã được cập nhật
                var updatedBooks = await _context.Books
                                              .Include(b => b.Category)
                                              .ToListAsync();

                ViewBag.Categories = _context.Categories.ToList();
                //return View("AddBook", updatedBooks); // Trả về view với danh sách sách đã cập nhật
                return RedirectToAction("AddBook", "Books");
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "An error occurred while adding the book.";

                // Truy vấn lại danh sách sách hiện tại
                var currentBooks = await _context.Books
                                               .Include(b => b.Category)
                                               .ToListAsync();

                ViewBag.Categories = _context.Categories.ToList();
                //return View("AddBook", currentBooks); // Trả về view với danh sách sách hiện tại
                return RedirectToAction("AddBook", "Books");
            }
        }

        // cập nhật book
        [HttpPost]
        [Route("updateBook")]
        public async Task<IActionResult> UpdateBook(Book book, IFormFile ImageFile)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            try
            {
                var existingBook = await _context.Books.FindAsync(book.BookId);
                if (existingBook == null)
                {
                    TempData["ErrorMessage"] = "Book not found!";
                    return RedirectToAction("AddBook", new { page = CurrentPage });
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingBook.Image))
                    {
                        var oldImagePath = Path.Combine(_env.WebRootPath, existingBook.Image.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    existingBook.Image = "/uploads/" + fileName;
                }

                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.CategoryId = book.CategoryId;
                existingBook.Price = book.Price;
                existingBook.Description = book.Description;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Book updated successfully!";
                return RedirectToAction("AddBook", new { page = CurrentPage });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "An error occurred while updating the book.";
                return RedirectToAction("AddBook", new { page = CurrentPage });
            }
        }


        //xóa book
        [HttpPost]
        [Route("deleteBook")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
            {
                TempData["ErrorMessage"] = "Book not found!";
                return RedirectToAction("AddBook");
            }
            else
            {
                _context.Books.Remove(existingBook);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Delete Successfully";
            }

            return RedirectToAction("AddBook");
        }


        //---------------------------------- 
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> BookDetail(int id)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 1)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }

            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null) return NotFound();

            var reviews = await _context.Reviews
                .Where(r => r.BookId == id)
                .Include(r => r.User)  // Load thông tin User
                .ToListAsync();

            double avgRating = reviews.Any() ? reviews.Average(r => r.Ratting) : 0;

            ViewBag.book = book;
            ViewBag.Reviews = reviews;
            ViewBag.AverageRating = Math.Round(avgRating, 1);

            return View(book); // Truyền book vào View để dễ truy cập
        }


        private async Task<List<Book>> GetRandomBooksAsync(int count = 4)
        {

            return await _context.Books
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
        }




    }
}
