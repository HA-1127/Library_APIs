namespace Library.Models
{
    public class ImageBook
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public Book book { get; set; } = null!;
        public string UrlImage { get; set; } = null!;
    }
}
