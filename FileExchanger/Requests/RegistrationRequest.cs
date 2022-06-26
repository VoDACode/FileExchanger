using System;
using System.ComponentModel.DataAnnotations;

namespace FileExchanger.Requests
{
    public class RegistrationRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public DateTime Ts { get; set; }
    }
}
