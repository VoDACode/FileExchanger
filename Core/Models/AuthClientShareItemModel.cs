using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class AuthClientShareItemModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public AuthClientModel Client { get; set; }
        [Required]
        public ShareItemModel ShareItem { get; set; }
        [Required]
        public bool CanWrite { get; set; }
    }
}
