using System.Diagnostics;
using BookStore.Dtos.OrderDto;
using BookStore.Filters;
using BookStore.Models;
using BookStore.Services;
using BookStore.Utils;
using chat_application_demo.Utils;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SQLitePCL;

namespace BookStore.Controllers.Checkout
{
    [TypeFilter(typeof(AuthenticationFilter))]
    public class CheckOutController : Controller
    {


        private readonly ILogger _logger;
        private readonly OrderService _service;
        private readonly IConfiguration _configuration;

        public CheckOutController(OrderService service, IConfiguration configuration, ILogger<CheckOutController> logger)
        {
            _service = service;
            _configuration = configuration;
            _logger = logger;

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

            // out of stock 
            if (order.OrderStatus == Enums.OrderStatus.OutOfStock)
            {
                return View();
            }

            string vnpayurl = _service.CreatePaymentUrl(order.OrderId, HttpContext);

            return Redirect(vnpayurl);
        }

        [HttpGet("/PaymentCallback")]
        public IActionResult PaymentCallback()
        {
            _logger.LogInformation("Begin VNPAY Return, URL={0}", Request.GetDisplayUrl());

            var vnPay = new VnPayLibrary();

            _logger.LogInformation("Request.Query list:");
            // Populate _responseData with query parameters
            foreach (var (key, value) in Request.Query)
            {
                vnPay._responseData[key] = value;  // Directly add to the dictionary
                Console.WriteLine("key: " + key + ", value: " + value);
            }

            string hash = _configuration["Vnpay:vnp_HashSecret"];

            bool isValid = vnPay.ValidateSignature(vnPay.GetResponseDataByKey("vnp_SecureHash"), hash);

            if (isValid)
            {
                // success set update 
                var order = _service.UpdatePreference(Request.GetDisplayUrl());
                string username = UserSessionManager.GetUserInfo(HttpContext).Username;
                int orderId = order.OrderId;
                return RedirectToAction("OrderDetail", new { username = username, orderId = orderId });
            }
            else
            {
                // fall set fail and roll back
                ViewBag.Sucess = isValid;
                return View();
            }

        }


        [Route("{username}/Order/{orderId}")]
        public IActionResult OrderDetail(string username, int orderId)
        {
            ViewBag.usename = username;
            ViewBag.orderId = orderId;
            if (UserSessionManager.GetUserInfo(HttpContext).Username.Equals(username))
            {

                var order = _service.GetById(orderId);
                var response = _service.ParseVnPayResponse(order.Preferences);


                ViewBag.order = order;
                ViewBag.response = response;

                return View();
            }
            return Redirect("/");
        }
    }
}
