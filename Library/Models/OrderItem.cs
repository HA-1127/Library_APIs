using Microsoft.EntityFrameworkCore;

namespace Library.Models
{
    [PrimaryKey(nameof(BookId), nameof(OrderId))]
    public class OrderItem
    {
        public int BookId { get; set; }
        public Book book { get; set; } = null!;
        public int OrderId { get; set; }
        public Order order { get; set; } = null!;
        public double Price { get; set; }
        public double Quantity { get; set; }

    }
}
