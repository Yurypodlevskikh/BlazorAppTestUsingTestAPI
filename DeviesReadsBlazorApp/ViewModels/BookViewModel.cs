namespace DeviesReadsBlazorApp.ViewModels
{
    public class BookViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public int HaveRead { get; set; }
        public int CurrentlyReading { get; set; }
        public int WantToRead { get; set; }
    }

}
