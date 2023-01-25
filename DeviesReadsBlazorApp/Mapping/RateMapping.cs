using DeviesReadsBlazorApp.Models;

namespace DeviesReadsBlazorApp.Mapping
{
    public static class RateMapping
    {
        public static RateBookDTO? ToDTO(this RateBook rateBook) => rateBook is null ? null : new RateBookDTO
        {
            BookId = rateBook.BookId,
            Rating = rateBook.Rating
        };
    }
}
