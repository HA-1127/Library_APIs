using Library.Data;
using Library.Models;
using Library.Repositoirs.IRepositoirs;

namespace Library.Repositoirs
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContexest context) : base(context)
        {
        }
    }
}
