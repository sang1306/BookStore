using BookStore.Models;

namespace BookStore.Services
{
    public class OrderService
    {
        private readonly Prn222BookshopContext _context;
        public OrderService(Prn222BookshopContext context)
        {
            _context = context; 
        }
        public List<BookStore.Models.Book> GetBooksByIds(List<int> listId) {
            return _context.Books.Where(b => listId.Contains(b.BookId)).ToList();
        }
    }
}
