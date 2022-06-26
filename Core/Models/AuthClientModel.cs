using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Models
{
    public class AuthClientModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public UserModel ExchangerUser { get; set; }
        public DateTime Ts { get; set; }
        public ICollection<RefreshTokenModel> RefreshTokens { get; set; }
        public bool Active { get; set; }

        public AuthClientModel()
        {
            RefreshTokens = new HashSet<RefreshTokenModel>();
        }
    }
}
