using Library.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class ApplicationDbContexest:IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContexest(DbContextOptions<ApplicationDbContexest>options):base(options)
        {

        }
       public DbSet<Category> categories { get; set; }
        public DbSet<Publisher> publishers { get; set; }
        public DbSet<Author> authors { get; set; }
        public DbSet<Book> books { get; set; }
        public DbSet<ImageBook> imageBooks { get; set; }
        public DbSet<UserOTP> userOTPs { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> orderItems { get; set; }

    }
}
