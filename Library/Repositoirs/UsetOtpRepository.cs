using Library.Data;
using Library.Models;
using Library.Repositoirs.IRepositoirs;

namespace Library.Repositoirs
{
    public class UsetOtpRepository : Repository<UserOTP> ,IUserOtpRespository
    {
        public UsetOtpRepository(ApplicationDbContexest context) : base(context)
        {
        }
    }
}
