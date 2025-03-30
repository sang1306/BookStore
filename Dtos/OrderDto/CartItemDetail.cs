using BookStore.Models;

namespace BookStore.Dtos.OrderDto
{
    public class CartItemDetail
    {
        public Models.Book Book { get; set; }
        public int Quantity { get; set; }

    }
}
