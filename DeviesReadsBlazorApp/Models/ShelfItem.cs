using System.ComponentModel.DataAnnotations;

namespace DeviesReadsBlazorApp.Models
{
    public class ShelfItem
    {
        public string BookId { get; set; }
        public string Status { get; set; }
    }

    public enum ShelfStatus 
    {
        [Display(Name = "Have Read")]
        haveRead,
        [Display(Name = "Currently Reading")]
        currentlyReading,
        [Display(Name = "Want To Read")]
        wantToRead }
}
