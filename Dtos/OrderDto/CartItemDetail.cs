using BookStore.Models;

namespace BookStore.Dtos.OrderDto
{
    public class CartItemDetail
    {
        public Book Book { get; set; }
        public int Quantity { get; set; }

    }
}
