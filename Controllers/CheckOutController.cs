using BookStore.Filters;
using BookStore.Models;
using BookStore.Services;
using chat_application_demo.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [TypeFilter(typeof(AuthenticationFilter))]
    public class CheckOutController : Controller
    {

        private readonly OrderService _service;
        public CheckOutController(OrderService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            // get info use from session 
            User user = UserSessionManager.GetUserInfo(HttpContext);

            // get cartItems from cookie

            // get string from cookie = "cart"
            string cartCookie = Request.Cookies["cart"];
            List<CartItem> cartItems = _service.ExtractCartItem(cartCookie);

            // set book from service
            List<BookStore.Models.Book> listBooks = _service.GetBooksByIds(cartItems.Select(c => c.BookId).ToList());
            // create dictionary
            List<CartItemDetails> booksWithQuanity = listBooks.Select(book =>
            {
                var cartItem = cartItems.FirstOrDefault(b => b.BookId == book.BookId);
                return new CartItemDetails
                {
                    Book = book,
                    Quantity = cartItem?.Quantity ?? 1
                };
            }).ToList();

            decimal subtotal = 0;
            foreach (var book in booksWithQuanity)
            {
                decimal onebook = book.Quantity * book.Book.Price.Value;
                subtotal += onebook;
            }


            decimal shipping = 35000;

            ViewBag.User = user;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + shipping;
            ViewBag.CartItemBook = booksWithQuanity;
            return View();
        }



    }
}
