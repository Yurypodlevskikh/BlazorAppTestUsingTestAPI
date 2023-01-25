namespace DeviesReadsBlazorApp.Models
{
    public class BookDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string CoverUrl { get; set; }
        public string Description { get; set; }
        public double AverageRating { get; set; }
        public int HaveRead { get; set; }
        public int CurrentlyReading { get; set; }
        public int WantToRead { get; set; }
        public int UserRating { get; set; }
    }

}
