using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using chat_application_demo.Utils;

namespace BookStore.Controllers.AdminController
{
    
    public class AdminController : Controller
    {
        private readonly Prn222BookshopContext _context;

        public AdminController(Prn222BookshopContext context)
        {
            _context = context;
        }

       
        
        public async Task<IActionResult> Index(int? page)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 3)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            int pageNumber = (page ?? 1); 
            int pageSize = 8;

            var users = await _context.Users.Include(u => u.RoleNavigation).ToListAsync();
            

            var pagedUsers = users.ToPagedList(pageNumber, pageSize);
            return View(pagedUsers);
        }
        

        public IActionResult Create()
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 3)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Password,Email,Role,Preferences,CreateAt,Address,Status")] User user)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 3)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            //if (ModelState.IsValid)
            //{
            _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.Role);
            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 3)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.Role);
            return View(user);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Password,Email,Role,Preferences,CreateAt,Address,Status")] User user)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 3)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            if (id != user.UserId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "RoleId", user.Role);
            return View(user);

        }


        public async Task<IActionResult> Delete(int? id)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 3)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userSession = UserSessionManager.GetUserInfo(HttpContext);
            if (userSession == null || userSession.Role != 3)
            {
                return RedirectToAction("AccessDenied", "Home"); // Chuyển hướng tới trang báo lỗi
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
