using DeviesReadsBlazorApp.Models;
using DeviesReadsBlazorApp.ViewModels;

namespace DeviesReadsBlazorApp.Data
{
    public interface IDeviesReadUser
    {
        Task<UserDTO> GetUserById(string UserId);
        Task<List<UserBookViewModel>> GetUsersBooks(string UserId);
        Task<bool> UserExists(string UserId);
        Task<string?> UserHasThisBook(IfBookOnShelf bookShelf);
        Task<bool> AddTheBookOnTheShelf(BookOnTheShelf bookShelf);
        Task<bool> UpdateBookOnTheShelf(BookOnTheShelf bookShelf);
    }
}
