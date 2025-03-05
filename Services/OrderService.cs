using BookStore.Controllers;
using BookStore.Dtos.OrderDto;
using BookStore.Models;
using BookStore.Utils;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class OrderService
    {

        private readonly ILogger _logger;
        private readonly Prn222BookshopContext _context;
        private readonly IConfiguration _configuration;
        public OrderService(Prn222BookshopContext context, IConfiguration configuration, ILogger<OrderService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;   
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
                Address = request.StreetAdress + ", " + request.City,
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


        public string CreatePaymentUrl(int orderId, HttpContext httpContext)
        {
            string vnp_Returnurl = _configuration["Vnpay:vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = _configuration["Vnpay:vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = _configuration["Vnpay:vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = _configuration["Vnpay:vnp_HashSecret"]; //Secret Key


            var order = GetById(orderId);
            if (order == null) return "";


            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary(_configuration);

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((int)(order.TotalAmount * 100)).ToString());

            //vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");


            vnpay.AddRequestData("vnp_CreateDate", order.OrderDate.Value.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", UtilsVnPay.GetIpAddress(httpContext));

            vnpay.AddRequestData("vnp_Locale", "vn");


            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            //vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày
            vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày


            string paymentUrl = vnpay.CreateRequestUrl();
            _logger.LogInformation("VNPAY URL: {0}", paymentUrl);
            return paymentUrl;

        }
    }
}
