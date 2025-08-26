using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Publisher
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string FristName { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string LastName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public ICollection<Book> books { get; set; } = new List<Book>();
    }
}
