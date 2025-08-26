using System.ComponentModel.DataAnnotations;

namespace Library.DTOS.author
{
    public class AuthorFilterDtos
    {
        public string? FirstName { get; set; } 
      
        public string? LastName { get; set; } 
       
        public string? Address { get; set; }
        public string? Email { get; set; }
        public double? FromAge { get; set; }
        public double? ToAge { get; set; }
    }
}
