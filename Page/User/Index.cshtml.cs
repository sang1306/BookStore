using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;



namespace BookStore.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly Prn222BookshopContext _context;

        public IndexModel(Prn222BookshopContext context)
        {

            _context = context;
        }

        public List<Models.User> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
        }
    }
}
