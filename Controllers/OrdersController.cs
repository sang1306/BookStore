using BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookStore.Controllers
{
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
            // parse cookie string to list id
            List<int> bookIds = new List<int>();
            if (!string.IsNullOrEmpty(carCookie))
            {
                bookIds = carCookie.Split(',')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(int.Parse)
                    .ToList();
            }
            List<BookStore.Models.Book> listBooks = _service.GetBooksByIds(bookIds);

            // send them to viewbag
            ViewBag.CartBooks = listBooks;
            return View();
        }
        [HttpPost("AddTocart")]
        public IActionResult AddToCart(int bookId)
        {
            // get string from cookie = "cart" 
            string cartCookie = Request.Cookies["cart"];
            //get id book
            List<int> bookIds = new List<int>();
            if (!string.IsNullOrEmpty(cartCookie))
            {
                bookIds = cartCookie.Split(',')
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(int.Parse)
                                   .ToList();
            }
            // add that id to string 
            if (!bookIds.Contains(bookId))
            {
                bookIds.Add(bookId);
            }
            string updateCarCookie = string.Join(",", bookIds);
            // cookie opptions 
            var cookiOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
            };
            Response.Cookies.Append("cart", updateCarCookie, cookiOptions);

            // send them to client => set string to cookie ="cart"
            return Json(new { success = true, cartCount = bookIds.Count });
        }
        [HttpPost("RemoveFromCart")]
        public IActionResult RemoveFromCart(int bookId)
        {
            // get string from cookie = "cart" 
            string cartCookie = Request.Cookies["cart"];
            //get id book
            List<int> bookIds = new List<int>();
            if (!string.IsNullOrEmpty(cartCookie))
            {
                bookIds = cartCookie.Split(',')
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(int.Parse)
                                   .ToList();
            }
            //remove that id to string 
            bookIds.Remove(bookId);

            string updateCarCookie = string.Join(",", bookIds);
            // cookie opptions 
            var cookiOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
            };
            Response.Cookies.Append("cart", updateCarCookie, cookiOptions);

            // send them to client => set string to cookie ="cart"
            return Json(new { success = true, cartCount = bookIds.Count });
        }
    }
}


