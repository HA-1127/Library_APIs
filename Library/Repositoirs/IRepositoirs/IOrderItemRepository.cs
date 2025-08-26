using Library.Migrations;
using Library.Models;

namespace Library.Repositoirs.IRepositoirs
{
    public interface IOrderItemRepository:IRepository<orderitem>
    {
         Task CreateRangeAsync(List<OrderItem> orderItems);
    }
}
