using Library.Data;
using Library.Migrations;
using Library.Models;
using Library.Repositoirs.IRepositoirs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Library.Repositoirs
{
    public class OrderItemRepository : Repository<orderitem>, IOrderItemRepository
    {
        
        public OrderItemRepository(ApplicationDbContexest context) : base(context)
        {
            Context = context;
        }

        public ApplicationDbContexest Context { get; }

        public async Task CreateRangeAsync(List<OrderItem> orderItems)
        {
            await Context.orderItems.AddRangeAsync(orderItems);
        }
    }
}
