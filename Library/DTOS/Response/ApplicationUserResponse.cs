namespace Library.DTOS.Response
{
    public class ApplicationUserResponse
    {

        public string Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset LockoutEnd { get; set; }
    }

}
