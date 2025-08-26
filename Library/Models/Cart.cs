using Microsoft.EntityFrameworkCore;

namespace Library.Models
{
    [PrimaryKey(nameof(ApplicationUserId), nameof(BookId))]
    public class Cart
    {
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser applicationUser = null!;
        public int  BookId { get; set; }
        public Book book { get; set; } = null!;
        public int Count { get; set; }


    }
}
