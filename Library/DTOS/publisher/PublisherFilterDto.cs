using System.ComponentModel.DataAnnotations;

namespace Library.DTOS.publisher
{
    public class PublisherFilterDto
    {
        public string? FristName { get; set; } 
       
        public string? LastName { get; set; } 
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
