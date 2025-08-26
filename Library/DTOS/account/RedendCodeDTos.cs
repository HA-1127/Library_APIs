using System.ComponentModel.DataAnnotations;

namespace Library.DTOS.account
{
    public class RedendCodeDTos
    {
        [Required]
        public string Code { get; set; } = string.Empty;
        public string UserId { get; set; } 
    }
}
