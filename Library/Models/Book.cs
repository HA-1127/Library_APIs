using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;
        public string? Titel { get; set; }
        public string? Isbn { get; set; }
        public string PublicationYare { get; set; } = string.Empty;
        [Range(0, 50_000)]
        public double Price { get; set; }
        [Range(0,50_0000)]
        public int Quantity { get; set; }
        [Range(0,100)]
        public double Discount { get; set; }
        public string? Discription { get; set; }
        [Range(0, 5)]
        public double Rate { get; set; }
        public int AuthorId { get; set; }
        public Author author { get; set; } = null!;
        public int PublisherId { get; set; }
        public Publisher publisher { get; set; } = null!;
        public int  CategoryId { get; set; }
        public Category category { get; set; } = null!;
        public ICollection<ImageBook> imageBooks { get; set; } = new List<ImageBook>();

    }
}
