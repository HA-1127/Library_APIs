namespace Library.Models
{
    public enum OrderStatus
    {
        pending,
        processing,
        shipped,
        inWay,
        completed,
        canceled,
        refunded
    }

    public enum PaymentMethod
    {
        Visa,
        Cash
    }
    public class Order
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser applicationUser { get; set; } = null!;
        public DateTime Date { get; set; }
        public double TotalPrice { get; set; }
        public int Count { get; set; }
        public OrderStatus orderStatus { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public string? Carrier { get; set; }
        public string? CarrierId { get; set; }
        //number to pay
        public string? TransactionId { get; set; }
        //attempt to pay
        public string? SessionId { get; set; }
    }
}
