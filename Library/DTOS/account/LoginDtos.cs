using System.ComponentModel.DataAnnotations;

namespace Library.DTOS.account
{
    public class LoginDtos
    {
        [Required]
        public string EmailORUserName { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
