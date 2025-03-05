using BookStore.Controllers;
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
        public List<BookStore.Models.Book> GetBooksByIds(List<int> listId)
        {
            return _context.Books.Where(b => listId.Contains(b.BookId)).ToList();
        }
        public List<CartItem> ExtractCartItem(string cookie)
        {
            // Parse cookie to cart items
            List<CartItem> cartItems = new List<CartItem>();
            if (!string.IsNullOrEmpty(cookie))
            {
                cartItems = cookie.Split(',')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(item =>
                    {
                        var parts = item.Split(':');
                        return new CartItem
                        {
                            BookId = int.Parse(parts[0]),
                            Quantity = parts.Length > 1 ? int.Parse(parts[1]) : 1
                        };
                    })
                    .ToList();
            }
            return cartItems;
        }


    }
}
