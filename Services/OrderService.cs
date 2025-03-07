using BookStore.Controllers;
using BookStore.Dtos.OrderDto;
using BookStore.Models;
using BookStore.Utils;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace BookStore.Services
{
    public class OrderService
    {

        private readonly ILogger _logger;
        private readonly Prn222BookshopContext _context;
        private readonly IConfiguration _configuration;

        private string vnp_Url; //URL thanh toan cua VNPAY 
        private string vnp_Api;
        private string vnp_TmnCode; //Ma định danh merchant kết nối (Terminal Id)
        private string vnp_HashSecret; //Secret Key
        private string vnp_Returnurl; //URL nhan ket qua tra ve 

        public OrderService(Prn222BookshopContext context, IConfiguration configuration, ILogger<OrderService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;

            vnp_Url = _configuration["Vnpay:vnp_Url"];
            vnp_Api = _configuration["Vnpay:vnp_Api"];
            vnp_TmnCode = _configuration["Vnpay:vnp_TmnCode"];
            vnp_HashSecret = _configuration["Vnpay:vnp_HashSecret"];
            vnp_Returnurl = _configuration["Vnpay:vnp_Returnurl"];
        }
        public List<BookStore.Models.Book> GetBooksByIds(List<int> listId)
        {
            return _context.Books.Where(b => listId.Contains(b.BookId)).ToList();
        }

        public Order GetById(int id)
        {
            return _context.Orders.Include(o => o.OrderDetails).FirstOrDefault(e => e.OrderId == id);
        }

        public Order CreateOrder(OrderRequest request, int userId, List<OrderDetail> details)
        {

            var orderdb = _context.Orders.Add(new Order()
            {
                UserId = userId,
                OrderStatus = Enums.OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                Address = request.StreetAdress + ", " + request.City,
                Phone = request.Phone,
                TotalAmount = request.TotalAmount,
                OrderDetails = details
            }).Entity;
            UpdateBookQuantity(details);

            // Check if stock is valid and update quantities
            bool isStockValid = UpdateBookQuantity(details);
            if (!isStockValid)
            {
                orderdb.OrderStatus = Enums.OrderStatus.OutOfStock;
                // Rollback book quantities
                RollbackBookQuantity(details);
            }
            _context.SaveChanges();
            return orderdb;
        }

        public bool UpdateBookQuantity(List<OrderDetail> details)
        {
            foreach (var item in details)
            {
                var book = _context.Books.FirstOrDefault(b => b.BookId == item.BookId);
                if (book == null || book.Stock < item.Quantity.Value)
                {
                    return false; // Not enough stock or book not found
                }
                book.Stock -= item.Quantity.Value;
            }
            return true;
        }

        public void RollbackBookQuantity(List<OrderDetail> details)
        {
            foreach (var item in details)
            {
                var book = _context.Books.FirstOrDefault(b => b.BookId == item.BookId);
                if (book != null)
                {
                    book.Stock += item.Quantity.Value; // Reverse the stock deduction
                }
            }
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


        public string CreatePaymentUrl(int orderId, HttpContext httpContext)
        {
            var order = GetById(orderId);
            if (order == null) return "";


            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)(order.TotalAmount * 100)).ToString());


            vnpay.AddRequestData("vnp_CreateDate", order.OrderDate.Value.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", UtilsVnPay.GetIpAddress(httpContext));

            vnpay.AddRequestData("vnp_Locale", "vn");


            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            //vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày


            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            _logger.LogInformation("VNPAY URL: {0}", paymentUrl);
            return paymentUrl;

        }

        public Order UpdatePreference(string url)
        {
            var response = ParseVnPayResponse(url);
            var order = _context.Orders.FirstOrDefault(e => e.OrderId == int.Parse(response.TxnRef));
            order.Preferences = url;
            _context.SaveChanges();
            return order;
        }
        public VnPayResponseQuery ParseVnPayResponse(string url)
        {
            var uri = new Uri(url);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            return new VnPayResponseQuery
            {
                Amount = long.Parse(queryParams["vnp_Amount"]),
                BankCode = queryParams["vnp_BankCode"],
                BankTranNo = queryParams["vnp_BankTranNo"],
                CardType = queryParams["vnp_CardType"],
                OrderInfo = Uri.UnescapeDataString(queryParams["vnp_OrderInfo"]),
                PayDate = DateTime.ParseExact(queryParams["vnp_PayDate"], "yyyyMMddHHmmss", null),
                ResponseCode = queryParams["vnp_ResponseCode"],
                TmnCode = queryParams["vnp_TmnCode"],
                TransactionNo = queryParams["vnp_TransactionNo"],
                TransactionStatus = queryParams["vnp_TransactionStatus"],
                TxnRef = queryParams["vnp_TxnRef"],
                SecureHash = queryParams["vnp_SecureHash"]
            };
        }

    }
}
