using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;
using X.PagedList;

namespace BookStore.Controllers.AdminController
{
    public class AdminController : Controller
    {
        private readonly Prn222BookshopContext _context;

        public AdminController(Prn222BookshopContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = (page ?? 1); 
            int pageSize = 8;

            var users = await _context.Users.Include(u => u.RoleNavigation).ToListAsync();

            var pagedUsers = users.ToPagedList(pageNumber, pageSize);
            return View(pagedUsers);
        }
    }
}
