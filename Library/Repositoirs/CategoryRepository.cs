using Library.Data;
using Library.Models;
using Library.Repositoirs.IRepositoirs;

namespace Library.Repositoirs
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContexest context) : base(context)
        {
        }
    }
}
