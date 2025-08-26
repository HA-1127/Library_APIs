using Library.DTOS.account;
using Library.Models;
using Library.Repositoirs.IRepositoirs;
using Library.Utility;
using Mapster;

using Microsoft.AspNetCore.Mvc;

using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountsController(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistersDtos registersDtos)
        {
            ApplicationUser applicationUser = registersDtos.Adapt<ApplicationUser>();
            var result = await _unitOfWork.UserManager.CreateAsync(applicationUser, registersDtos.Password);

            if (result.Succeeded)
            {
                // Send Email
                var token = await _unitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                var link = Url.Action(nameof(ConfirmEmail), "Accounts", new { area = "Identity", token = token, userId = applicationUser.Id }, Request.Scheme);
                await _unitOfWork.EmailSender.SendEmailAsync(registersDtos.Email, "Confirm Your Account", $"<h1>Confirm Your Account By Clicking <a href='{link}'>Here</a></h1>");

                // Send msg
                //var redirectLink = Url.Action(nameof(Index), "Home", new { area = "Customer" }, Request.Scheme);

                //return Created(redirectLink, "Add Account Successfully, Confirm Your Account!");

                //return Created();
                await _unitOfWork.UserManager.AddToRoleAsync(applicationUser, SD.Customer);


                return Ok("Add Account Successfully, Confirm Your Account!");

                //return Ok();
            }

            return BadRequest(result.Errors);
        }
        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegistersDtos registersAdminDtos)
        {
            ApplicationUser applicationUser = registersAdminDtos.Adapt<ApplicationUser>();

            var result = await _unitOfWork.UserManager.CreateAsync(applicationUser, registersAdminDtos.Password);
            if (result.Succeeded)
            {
                await _unitOfWork.UserManager.AddToRoleAsync(applicationUser, SD.Admin);
                await _unitOfWork.SignInManager.SignInAsync(applicationUser, false);
                return Ok("Successfull Account and login ");
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(userId);

            if (user is not null)
            {
                var result = await _unitOfWork.UserManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                    return Ok();

                return BadRequest(result.Errors);
            }

            return NotFound();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDtos loginDtos)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(loginDtos.EmailORUserName) ??
                await _unitOfWork.UserManager.FindByNameAsync(loginDtos.EmailORUserName);
            if (user is not null)
            {

                var result = await _unitOfWork.SignInManager.PasswordSignInAsync(user.UserName, loginDtos.Password, loginDtos.RememberMe, lockoutOnFailure: true);

                if (result.IsLockedOut)
                {
                    return BadRequest("Too Many Attempts");
                }

                if (result.Succeeded)
                {
                    if (!user.EmailConfirmed)
                    {
                        return BadRequest("Confirm Your Account!");
                    }

                    if (!user.LockoutEnabled)
                    {
                        return BadRequest($"You have a block till {user.LockoutEnd}");
                    }
                    var userRoles = await _unitOfWork.UserManager.GetRolesAsync(user);

                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, String.Join(",", userRoles)),
                    };

                    var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EraaSoft515##EraaSoft515##EraaSoft515##EraaSoft515##")), SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: "https://localhost:7135",
                        audience: "https://localhost:5000,https://localhost:5500,https://localhost:4200",
                        claims: claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: signingCredentials
                    );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expires = token.ValidTo
                    });
                }
            }

            return BadRequest("Invalid User Name Or Password");
        }
        [HttpPost("ResendEmailConfirmaion")]
        public async Task<IActionResult> ResendEmailConfirmaion(ResendEmailComfrimaionDtos resendEmailComfrimaionDtos)

        {
            var user = await _unitOfWork.UserManager.FindByNameAsync(resendEmailComfrimaionDtos.EmailOrName) ??
                await _unitOfWork.UserManager.FindByEmailAsync(resendEmailComfrimaionDtos.EmailOrName);
            if (user is not null)
            {
                var token = await _unitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", token = token, userId = user.Id }, Request.Scheme);
                await _unitOfWork.EmailSender.SendEmailAsync(user.Email!, "Confirm Your Account", $"<h1>Confirm Your Account By Clicking <a href='{link}'>Here</a></h1>");

                return Ok("Confirm Your Account Again!");
            }

            return NotFound();
        }
        [HttpPost("ForgetPasswod")]
        public async Task<IActionResult> ForgetPasswod(ForgetPasswordDTos forgetPasswordDTos)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(forgetPasswordDTos.EmailOrUserName) ??
                await _unitOfWork.UserManager.FindByNameAsync(forgetPasswordDTos.EmailOrUserName);


            var userotp = await _unitOfWork.UserOtpRespository
                .GetAsync(e => e.ApplicationUserId == user.Id);
            
            var otpNUmber = userotp.Count(e=>(e.Date.Day == DateTime.UtcNow.Day) &&
            (e.Date.Month == DateTime.UtcNow.Month) && (e.Date.Year == DateTime.UtcNow.Year));
             if (otpNUmber < 4)
            {
                if (user is not null)
                {
                    var OTPNumber = new Random().Next(1000, 9999);
                    await _unitOfWork.EmailSender.SendEmailAsync(user.Email!, "Reset Password", $"<h1>Reset Password Using OTP Number {OTPNumber}</h1>");

                    await _unitOfWork.UserOtpRespository.CreateAsync(new()
                    {
                        Code = OTPNumber.ToString(),
                        Date = DateTime.UtcNow,
                        ExpirationDate = DateTime.UtcNow.AddHours(1),
                        ApplicationUserId = user.Id
                    });
                    await _unitOfWork.UserOtpRespository.CommitAsync();
                    return RedirectToAction(nameof(ResetPassword), "Accounts", new { area = "Identity", userid = user.Id }, "Successfull resend cod");
                }
            }
         
            return BadRequest("Too Many Request, Please try again Later");
        }
        
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(RedendCodeDTos redendCodeDTos)
        {
            var userOTP = (await _unitOfWork.UserOtpRespository.GetAsync(e => e.ApplicationUserId ==redendCodeDTos.UserId)).OrderBy(e => e.Id).LastOrDefault();

            if (userOTP is not null)
            {
                if (DateTime.UtcNow < userOTP.ExpirationDate && !userOTP.Status && userOTP.Code == redendCodeDTos.Code)
                {
                    return CreatedAtAction(nameof(ChangePassword), "Accounts", new { area = "Identity", userId = userOTP.ApplicationUserId! }, string.Empty);
                }
            }

            return BadRequest("Invalid Code");
        }
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTos changePasswordDTos)
        {
            var user = await _unitOfWork.UserManager.FindByIdAsync(changePasswordDTos.UserId);

            if (user is not null)
            {
                var token = await _unitOfWork.UserManager.GeneratePasswordResetTokenAsync(user);
                await _unitOfWork.UserManager.ResetPasswordAsync(user, token, changePasswordDTos.Password);

                return Ok("Reset Password Successfully");
            }

            return NotFound();
        }
    }
}
