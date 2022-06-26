using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class RefreshTokenModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; }
        public string TokenSalt { get; set; }
        public DateTime Ts { get; set; }
        public DateTime ExpiryDate { get; set; }
        public virtual AuthClientModel User { get; set; }

    }
}
