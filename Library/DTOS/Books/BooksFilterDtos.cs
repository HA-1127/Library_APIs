using System.ComponentModel.DataAnnotations;

namespace Library.DTOS.Books
{
    public class BooksFilterDtos
    {
        
        public string? Name { get; set; } 
       
        public string? PublicationYare { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }

        public bool isHot { get; set; }

        public string? AuthorNme { get; set; }

        public string? PublisherName { get; set; }

        public int? CategoryId { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
