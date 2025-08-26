using System.ComponentModel.DataAnnotations;

namespace Library.DTOS.Books
{
    public class BooksCreateDtos
    {
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        public string? Titel { get; set; }
        public string? Isbn { get; set; }
        public string PublicationYare { get; set; } = string.Empty;
        [Range(0, 50_000)]
        public double Price { get; set; }
        [Range(0, 50_0000)]

        public double Discount { get; set; }
        public string? Discription { get; set; }
        [Range(0, 5)]

        public int AuthorId { get; set; }

        public int PublisherId { get; set; }

        public int CategoryId { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
