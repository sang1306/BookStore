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


            Response.Cookies.Delete("cart");

            //User user = UserSessionManager.GetUserInfo(HttpContext);

            Order order = _service.CreateOrder(orderRequest, user.UserId, orderDetails);

            Console.WriteLine(order);

            // out of stock 
            if (order.OrderStatus == Enums.OrderStatus.OutOfStock)
            {
                string username = UserSessionManager.GetUserInfo(HttpContext).Username;
                return RedirectToAction("OrderDetail", new { username = username, orderId = order.OrderId });
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
                // check status code 
                // success set order status is paid
                // fail rollback quantity  , set order status is fail
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
            //Request.Cookies["card"].Expires = DateTime.Now.AddDays(-1);

            if (UserSessionManager.GetUserInfo(HttpContext).Username.Equals(username))
            {
                // username doesn't match
                if (!UserSessionManager.GetUserInfo(HttpContext).Username.Equals(username))
                {
                    return Redirect("/");
                }

                var order = _service.GetById(orderId);
                ViewBag.order = order;

                // out of stock orders
                if (order.OrderStatus == Enums.OrderStatus.OutOfStock)
                {
                    ViewBag.status = false;
                    ViewBag.message = "Some book is out of order sorry";
                    return View();
                }

                var response = _service.ParseVnPayResponse(order.Preferences);
                ViewBag.response = response;
                ViewBag.message = _configuration[$"VnpayResponseCodes:{response.ResponseCode}"];

                // status check 
                string[] successCodes = { "00", "07" };
                ViewBag.status = successCodes.Contains(response.ResponseCode);

                return View();

            }
            return Redirect("/");
        }

        [Route("{username}/Order")]
        public IActionResult OrderView(string username, int page = 1, int pageSize = 5)
        {
            ViewBag.username = username;

            var user = UserSessionManager.GetUserInfo(HttpContext);
            var allOrders = _service.GetListOrder(user.UserId);

            // Pagination logic
            var paginatedOrders = allOrders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.orders = paginatedOrders;
            ViewBag.currentPage = page;
            ViewBag.totalPages = (int)Math.Ceiling((double)allOrders.Count / pageSize);

            return View();
        }
    }
}
