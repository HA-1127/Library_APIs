using Library.Models;
using Library.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Library.Repositoirs.IRepositoirs
{
    
    public interface IUnitOfWork :IDisposable
    {
        public IAuthorRepository AuthorRepository { get; }
        public IBooksRepository BooksRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IPublisharRepository PublisharRepository { get; }
        public IImageBooksRepository ImageBooksRepository { get; }
        public IUserOtpRespository UserOtpRespository { get; }
        public ICartRepository CartRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderItemRepository OrderItemRepository { get; }
        public UserManager<ApplicationUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }
        public SignInManager<ApplicationUser> SignInManager { get; }
        public IEmailSender EmailSender { get; }

    }
}
