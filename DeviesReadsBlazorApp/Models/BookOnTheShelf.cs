namespace DeviesReadsBlazorApp.Models
{
    public class BookOnTheShelf
    {
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string BookId { get; set; }
        public string Status { get; set; }
    }

    public class IfBookOnShelf
    {
        public string UserId { get; set; }
        public string BookId { get; set; }
    }
}
