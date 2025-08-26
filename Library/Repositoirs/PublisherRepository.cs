using Library.Data;
using Library.Models;
using Library.Repositoirs.IRepositoirs;

namespace Library.Repositoirs
{
    public class PublisherRepository : Repository<Publisher>, IPublisharRepository
    {
        public PublisherRepository(ApplicationDbContexest context) : base(context)
        {
        }
    }
}
