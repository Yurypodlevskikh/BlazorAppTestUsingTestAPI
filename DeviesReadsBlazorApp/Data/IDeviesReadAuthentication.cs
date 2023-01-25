using DeviesReadsBlazorApp.Models;

namespace DeviesReadsBlazorApp.Data
{
    public interface IDeviesReadAuthentication
    {
        Task<UserAuthResponseDTO> Register(RegisterDTO user);
        Task<UserLocalStorage> Login(RegisterDTO user);
        Task<bool> IsLoggedIn(string accessToken);
    }
}
