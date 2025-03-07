namespace BookStore.Enums
{
    public enum OrderStatus
    {
        Pending = 0,        // Chờ thanh toán
        Paid = 1,           // Đã thanh toán
        Processing = 2,     // Đang xử lý
        Shipped = 3,        // Đã giao hàng
        Completed = 4,      // Hoàn tất
        Cancelled = 5,      // Đã hủy
        Refunded = 6,        // Đã hoàn tiền
        OutOfStock = 7        // Hết hàng
    }
}
