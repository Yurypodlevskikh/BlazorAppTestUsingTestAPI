using System.ComponentModel.DataAnnotations;

namespace DeviesReadsBlazorApp.Models
{
    public class UserLocalStorage
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? AccessToken { get; set; }
    }
}
