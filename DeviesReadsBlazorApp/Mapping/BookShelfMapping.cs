using DeviesReadsBlazorApp.Models;

namespace DeviesReadsBlazorApp.Mapping
{
    public static class BookShelfMapping
    {
        public static ShelfItem? ToDTO(this BookOnTheShelf bookShelf) => bookShelf is null ? null : new ShelfItem
        {
            BookId = bookShelf.BookId,
            Status = bookShelf.Status
        };
    }
}
