using Library.Data;
using Library.Repositoirs.IRepositoirs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace Library.Utility.DBinitializer
{
    public class Dbinitializer : IDBInitializer
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContexest _dbContexest;

        public Dbinitializer(IUnitOfWork unitOfWork, ApplicationDbContexest dbContexest)
        {
            _unitOfWork = unitOfWork;
            _dbContexest = dbContexest;
        }

        public void Initialize()
        {
            try
            {
                if (_dbContexest.Database.GetPendingMigrations().Any())
                {
                    _dbContexest.Database.Migrate();
                }
                if (_unitOfWork.RoleManager is not null)
                {
                    _unitOfWork.RoleManager.CreateAsync(new(SD.SuperAdmin)).GetAwaiter().GetResult();
                    _unitOfWork.RoleManager.CreateAsync(new(SD.Admin)).GetAwaiter().GetResult();
                    _unitOfWork.RoleManager.CreateAsync(new(SD.Customer)).GetAwaiter().GetResult();
                    _unitOfWork.RoleManager.CreateAsync(new(SD.Employee)).GetAwaiter().GetResult();
                    _unitOfWork.RoleManager.CreateAsync(new(SD.Company)).GetAwaiter().GetResult();
                }
                _unitOfWork.UserManager.CreateAsync(new()
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "SuperAdmin",
                    Email = "hsn768972@gmail.com",
                    Address = "Desoke",
                    EmailConfirmed = true
                }, "Hassan123#").GetAwaiter().GetResult();
                var user = _unitOfWork.UserManager.FindByNameAsync("SuperAdmin").GetAwaiter().GetResult();

                _unitOfWork.UserManager.AddToRoleAsync(user, SD.SuperAdmin).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errores :{ex.Message}");
            }
           
        }

       
    }
}
