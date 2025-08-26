using Library.DTOS.Response;
using Library.Repositoirs.IRepositoirs;
using Mapster;
using Microsoft.AspNetCore.Mvc;
namespace Library.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class UsresController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsresController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var users = _unitOfWork.UserManager.Users;

            return Ok(users.Adapt<ApplicationUserResponse>());
        }
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            if (user is not null)
            {
                return Ok(user.Adapt<ApplicationUserResponse>());
            }
            return NotFound();
        }
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delate(string id)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            if (user is not null)
            {
               await _unitOfWork.UserManager.DeleteAsync(user);
                return Ok("Successfull Delete User");
            }

            return NotFound();
        }
        [HttpPatch("LockedUNLocked/{id}")]
        public async Task<IActionResult> LockedUNLocked(string id)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(id);
            if (user is not null)
            {
                if (user.LockoutEnabled)
                {
                    user.LockoutEnabled = false;
                    user.LockoutEnd = null;
                }
                else
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTime.UtcNow.AddDays(2);
                }
              await  _unitOfWork.UserManager.UpdateAsync(user);
                return Ok("update successfull");
            }
            return NotFound();
        }
    }
}
