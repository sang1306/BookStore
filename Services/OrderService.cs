using BookStore.Controllers;
using BookStore.Dtos.OrderDto;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

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
        public Order GetById(int id)
        {
            return _context.Orders.FirstOrDefault(e => e.OrderId == id);
        }
        public Order CreateOrder(OrderRequest request, int userId, List<OrderDetail> detais)
        {

            var orderdb = _context.Orders.Add(new Order()
            {
                UserId = userId,
                OrderStatus = Enums.OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                Address = request.StreetAdress +", " + request.City,
                Phone = request.Phone,
                TotalAmount = request.TotalAmount,
                OrderDetails = detais
            }).Entity;
            _context.SaveChanges();
            return orderdb;
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
