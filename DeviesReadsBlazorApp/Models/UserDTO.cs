namespace DeviesReadsBlazorApp.Models
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public Shelf[] Shelf { get; set; }
    }

    public class Shelf
    {
        public string BookId { get; set; }
        public string Status { get; set; }
    }
}
