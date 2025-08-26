using Library.Data;
using Library.Models;
using Library.Repositoirs.IRepositoirs;

namespace Library.Repositoirs
{
    public class ImageBookRepository : Repository<ImageBook>, IImageBooksRepository
    {
        public ImageBookRepository(ApplicationDbContexest context) : base(context)
        {
        }
    }
}
