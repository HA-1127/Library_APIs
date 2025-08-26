namespace Library.DTOS.category
{
    public class categoryUpdateDtos
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Discription { get; set; }
        public bool Status { get; set; }
    }
}
