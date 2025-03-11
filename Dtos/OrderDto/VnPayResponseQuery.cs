namespace BookStore.Dtos.OrderDto
{
    public class VnPayResponseQuery
    {
        public long? Amount { get; set; }
        public string? BankCode { get; set; }
        public string? BankTranNo { get; set; }
        public string? CardType { get; set; }
        public string? OrderInfo { get; set; }
        public DateTime? PayDate { get; set; }
        public string? ResponseCode { get; set; }
        public string? TmnCode { get; set; }
        public string? TransactionNo { get; set; }
        public string? TransactionStatus { get; set; }
        public string? TxnRef { get; set; }
        public string? SecureHash { get; set; }
    }
}
