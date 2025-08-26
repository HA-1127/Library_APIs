using System.ComponentModel.DataAnnotations;

namespace Library.DTOS.account
{
    public class ResendEmailComfrimaionDtos
    {
        [Required]
        public string EmailOrName { get; set; } = string.Empty;
    }
}
