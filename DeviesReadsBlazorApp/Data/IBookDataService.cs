using DeviesReadsBlazorApp.Models;
using DeviesReadsBlazorApp.ViewModels;

namespace DeviesReadsBlazorApp.Data
{
    public interface IBookDataService
    {
        Task<List<BookViewModel>> GetAllBooksAsync(string? sortParam);
        Task<List<BookViewModel>> GetBooksByGenreAsync(string? bookGenre);
        Task <BookDTO> GetBookByIdAsync(string Id);
        Task<double?> RateTheBook(RateBook rateBook);
    }
}
