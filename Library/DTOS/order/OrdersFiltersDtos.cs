using Library.Models;

namespace Library.DTOS.order
{
    public class OrdersFiltersDtos
    {
        public string? UserName { get; set; }
        public OrderStatus? orderStatus { get; set; }
        public PaymentMethod? paymentMethod { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? Todate { get; set; }

    }
}
