using Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class ShareItemModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public ItemType ItemType { get; set; }
        [Required]
        public string ShaeKey { get; set; }
        [Required]
        public int ShareObjectId { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public AuthClientModel Owner { get; set; }

        public bool CanWrite { get; set; } = false;
        public bool IsRequiredLogin { get; set; } = false;
        public bool AllUsers { get; set; } = true;
    }
}
