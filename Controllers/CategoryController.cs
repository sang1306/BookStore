using BookStore.Models;
using chat_application_demo.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace BookStore.Controllers
{
    [Route("/Category")]
    public class CategoryController : Controller
    {
        private readonly Prn222BookshopContext _context;
        private readonly IWebHostEnvironment _env;
        private int CurrentPage
        {
            get => HttpContext.Session.GetInt32("CurrentPage") ?? 1;
            set => HttpContext.Session.SetInt32("CurrentPage", value);
        }

        public CategoryController(Prn222BookshopContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Hiển thị danh sách thể loại
        [Route("viewCategories")]
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 3)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            var categories = await _context.Categories.ToListAsync();
            int pageNumber = (page ?? 1);
            int pageSize = 8;
            var pagedCategories = categories.ToPagedList(pageNumber, pageSize);
            return View(pagedCategories);
        }
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View(); // Trả về form edit với dữ liệu đã có
        }
        // Xử lý thêm thể loại sách 
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(Category category)
        {
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
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				TempData["ErrorMessage"] = "Không tìm thấy thể loại!";
				return RedirectToAction("Index");
			}
			return View(category); // Trả về form edit với dữ liệu đã có
		}
		[HttpPost("EditCategory")]
		public async Task<IActionResult> EditCategory(Category category)
        {
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
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Thể loại không tồn tại!";
                return RedirectToAction("Index");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa thể loại thành công!";
            return View(category);
        }

        //xóa thể loại
        [HttpPost]
        [Route("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
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
    }
}
