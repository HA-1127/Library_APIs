using Library.Data;
using Library.Models;
using Library.Repositoirs.IRepositoirs;

namespace Library.Repositoirs
{
    public class BooksRepository : Repository<Book> ,IBooksRepository
    {
        public BooksRepository(ApplicationDbContexest context) : base(context)
        {

        }
    }
}
