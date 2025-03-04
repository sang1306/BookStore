using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookStore.Controllers
{
    public class CartItem
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
    public class CartItemDetails
    {
        public Book Book { get; set; }
        public int Quantity { get; set; }
    }



    public class OrdersController : Controller
    {
        private readonly OrderService _service;
        public OrdersController(OrderService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/Cart")]
        public IActionResult Cart()
        {
            // get string from cookie = "cart"
            string carCookie = Request.Cookies["cart"];

            // parse cookie string to list [BookId:Quantity]
            List<CartItem> cartItems = extractCartItem(carCookie);


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


            // send them to viewbag
            ViewBag.CartBooks = booksWithQuanity;
            return View();
        }
        private List<CartItem> extractCartItem(string cookie)
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

        [HttpPost("AddTocart")]
        public IActionResult AddToCart(int bookId, int quantity = 1)
        {
            // Get string from cookie = "cart" 
            string cartCookie = Request.Cookies["cart"];

            var cartItems = extractCartItem(cartCookie);

            // Check if book already exists in cart
            var existingItem = cartItems.FirstOrDefault(item => item.BookId == bookId);
            if (existingItem != null)
            {
                // Update quantity if book already exists
                existingItem.Quantity += quantity;
            }
            else
            {
                // Add new cart item if book doesn't exist
                cartItems.Add(new CartItem { BookId = bookId, Quantity = quantity });
            }


            // Convert cart items back to cookie string format (BookId:Quantity)
            string updatedCartCookie = string.Join(",",
                cartItems.Select(item => $"{item.BookId}:{item.Quantity}"));

            // Set cookie options
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = false,
            };

            // Update cookie
            Response.Cookies.Append("cart", updatedCartCookie, cookieOptions);

            // Calculate total items count (sum of all quantities)
            int totalItemsCount = cartItems.Sum(item => item.Quantity);

            // Return JSON response
            return Json(new { success = true, cartCount = totalItemsCount });


        }

        [HttpPost("RemoveFromCart")]
        public IActionResult RemoveFromCart(int bookId)
        {
            // Get string from cookie = "cart" 
            string cartCookie = Request.Cookies["cart"];

            // Parse cookie to cart items
            List<CartItem> cartItems = new List<CartItem>();
            cartItems = extractCartItem(cartCookie);

            // Remove the item with the specified bookId
            cartItems.RemoveAll(item => item.BookId == bookId);

            // Convert cart items back to cookie string
            string updatedCartCookie = string.Join(",",
                cartItems.Select(item => $"{item.BookId}:{item.Quantity}"));

            // Set cookie options
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = false,
            };

            // Update cookie
            Response.Cookies.Append("cart", updatedCartCookie, cookieOptions);

            // Calculate total items
            int totalItemsCount = cartItems.Sum(item => item.Quantity);

            // Return JSON response
            return Json(new { success = true, cartCount = totalItemsCount });


        }

        [HttpPost("UpdateCartQuantity")]
        public IActionResult UpdateCartQuantity(int bookId, int quantity)
        {
            if (quantity <= 0)
            {
                // If quantity is zero or negative, remove the item
                return RemoveFromCart(bookId);
            }

            // Get string from cookie = "cart" 
            string cartCookie = Request.Cookies["cart"];

            // Parse cookie to cart items
            List<CartItem> cartItems = new List<CartItem>();
            cartItems = extractCartItem(cartCookie);


            // Find and update item quantity
            var existingItem = cartItems.FirstOrDefault(item => item.BookId == bookId);
            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
            }

            // Convert cart items back to cookie string
            string updatedCartCookie = string.Join(",",
                cartItems.Select(item => $"{item.BookId}:{item.Quantity}"));

            // Set cookie options
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = false,
            };

            // Update cookie
            Response.Cookies.Append("cart", updatedCartCookie, cookieOptions);

            // Calculate total items
            int totalItemsCount = cartItems.Sum(item => item.Quantity);

            // Get the book details to return updated price
            var book = _service.GetBooksByIds(new List<int> { bookId }).FirstOrDefault();
            decimal itemTotal = book != null ? book.Price.Value * quantity : 0;

            // Return JSON response
            return Json(new
            {
                success = true,
                cartCount = totalItemsCount,
                itemTotal = itemTotal
            });
        }
    }
}


