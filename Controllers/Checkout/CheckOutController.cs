using System.Diagnostics;
using BookStore.Dtos.OrderDto;
using BookStore.Filters;
using BookStore.Models;
using BookStore.Services;
using BookStore.Utils;
using chat_application_demo.Utils;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace BookStore.Controllers.Checkout
{
    [TypeFilter(typeof(AuthenticationFilter))]
    public class CheckOutController : Controller
    {


        private readonly OrderService _service;
        private readonly IConfiguration _configuration;
        public CheckOutController(OrderService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            // get info use from session 
            User user = UserSessionManager.GetUserInfo(HttpContext);

            List<CartItemDetail> booksWithQuantity = GetListCardItemDetail();

            decimal subtotal = 0;
            foreach (var book in booksWithQuantity)
            {
                decimal onebook = book.Quantity * book.Book.Price.Value;
                subtotal += onebook;
            }


            decimal shipping = 35000;

            ViewBag.User = user;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + shipping;
            ViewBag.CartItemBook = booksWithQuantity;
            return View();
        }
        private List<CartItemDetail> GetListCardItemDetail()
        {
            // get cartItems from cookie
            // get string from cookie = "cart"
            string cartCookie = Request.Cookies["cart"];
            List<CartItem> cartItems = _service.ExtractCartItem(cartCookie);

            // set book from service
            List<Book> listBooks = _service.GetBooksByIds(cartItems.Select(c => c.BookId).ToList());
            // create dictionary
            List<CartItemDetail> booksWithQuanity = listBooks.Select(book =>
            {
                var cartItem = cartItems.FirstOrDefault(b => b.BookId == book.BookId);
                return new CartItemDetail
                {
                    Book = book,
                    Quantity = cartItem?.Quantity ?? 1
                };
            }).ToList();
            return booksWithQuanity;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(OrderRequest orderRequest)
        {
            User user = UserSessionManager.GetUserInfo(HttpContext);

            if (!ModelState.IsValid)
            {
                // get info use from session 

                List<CartItemDetail> booksWithQuantity = GetListCardItemDetail();

                decimal subtotal = 0;
                foreach (var book in booksWithQuantity)
                {
                    decimal onebook = book.Quantity * book.Book.Price.Value;
                    subtotal += onebook;
                }
                decimal shipping = 35000;

                ViewBag.User = user;
                ViewBag.Subtotal = subtotal;
                ViewBag.Total = subtotal + shipping;
                ViewBag.CartItemBook = booksWithQuantity;

                return View("Index");
            }

            Console.WriteLine($"Order received from {orderRequest.FirstName} {orderRequest.LastName}");

            // set list orderdetail
            List<CartItemDetail> books = GetListCardItemDetail();

            List<OrderDetail> orderDetails = books.Select(cart =>
            {
                return new OrderDetail()
                {
                    BookId = cart.Book.BookId,
                    Quantity = cart.Quantity,
                    Subtotal = cart.Book.Price * cart.Quantity
                };
            }).ToList();

            //User user = UserSessionManager.GetUserInfo(HttpContext);

            Order order = _service.CreateOrder(orderRequest, user.UserId, orderDetails);

            Console.WriteLine(order);

            return RedirectToAction("Index", "Home");
        }
        
        private string CreatePaymentUrl(int orderId)
        {
            var order = _service.GetById(orderId);
            if (order == null) return "";

            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", ((int)(order.TotalAmount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng: " + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:ReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            string paymentUrl = vnpay.CreateRequestUrl(_configuration["Vnpay:Url"], _configuration["Vnpay:HashSecret"]);
            return paymentUrl;
        }


        
    }
}
