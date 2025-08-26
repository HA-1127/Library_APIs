using Library.Data;
using Library.Models;
using Library.Repositoirs.IRepositoirs;
using Library.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Library.Repositoirs
{
    public class UnitOfWork :IUnitOfWork
    {
        private readonly ApplicationDbContexest _dbContexest;

        public UnitOfWork(ApplicationDbContexest dbContexest,
            IAuthorRepository authorRepository,
            IBooksRepository booksRepository,
            ICategoryRepository categoryRepository,
            IPublisharRepository publisharRepository,
            IImageBooksRepository imageBooksRepository,
            IUserOtpRespository userOtpRespository,
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager <IdentityRole>roleManager,
            SignInManager<ApplicationUser>signInManager,
            IEmailSender emailSender
            )
        {
            _dbContexest = dbContexest;
            AuthorRepository = authorRepository;
            BooksRepository = booksRepository;
            CategoryRepository = categoryRepository;
            PublisharRepository = publisharRepository;
            ImageBooksRepository = imageBooksRepository;
            UserOtpRespository = userOtpRespository;
            CartRepository = cartRepository;
            OrderRepository = orderRepository;
            OrderItemRepository = orderItemRepository;
            UserManager = userManager;
            RoleManager = roleManager;
            SignInManager = signInManager;
            EmailSender = emailSender;
        }

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

        public void Dispose()
        {
            //throw new NotImplementedException();
            _dbContexest.Dispose();
        }
    }
}
