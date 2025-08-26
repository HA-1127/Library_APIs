using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        public string? Discription { get; set; }
        public bool Status { get; set; }
        public ICollection<Book> books { get; set; } = new List<Book>();
    }
}
