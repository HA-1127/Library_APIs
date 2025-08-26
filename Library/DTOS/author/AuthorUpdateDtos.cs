using System.ComponentModel.DataAnnotations;

namespace Library.DTOS.author
{
    public class AuthorUpdateDtos
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string LastName { get; set; } = string.Empty;
        public string? biography { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public double? Age { get; set; }
        public IFormFile? Image { get; set; } 
    }
}
