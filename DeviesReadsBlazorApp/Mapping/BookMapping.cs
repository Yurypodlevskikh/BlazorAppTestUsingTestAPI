using DeviesReadsBlazorApp.Models;
using DeviesReadsBlazorApp.ViewModels;

namespace DeviesReadsBlazorApp.Mapping
{
    public static class BookMapping
    {
        public static BookViewModel ToView(this BookDTO books) => new BookViewModel
        {
            Id = books.Id,
            Name = books.Name,
            Genre = books.Genre,
            HaveRead = books.HaveRead,
            CurrentlyReading = books.CurrentlyReading,
            WantToRead = books.WantToRead,
        };

        public static IEnumerable<BookViewModel> ToView(this List<BookDTO> books) => books.Select(ToView);
    }
}
