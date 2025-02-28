using System.Diagnostics;
using BookStore.Filters;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN222_Project_1.Models;

namespace PRN222_Project_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Prn222BookshopContext _context;

        public HomeController(ILogger<HomeController> logger, Prn222BookshopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var listBooks = await GetRandomBooksAsync(6);
            ViewBag.Books = listBooks;

            var listBooksNewArivall = await GetRecentBooksAsync(6);
            ViewBag.ListBooksNewArivall = listBooksNewArivall;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ErrorView()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<List<Book>> GetRandomBooksAsync(int count = 4)
        {
            return await _context.Books
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
        }
        private async Task<List<Book>> GetRecentBooksAsync(int count = 4)
        {
            return await _context.Books
                .OrderByDescending(x=> x.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
