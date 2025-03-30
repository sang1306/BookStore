namespace BookStore.Dtos.Book
{
    public class ReviewDTO
    {
        public int BookId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }


}
