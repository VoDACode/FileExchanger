using Core.Models;
using System;

namespace FileExchanger.Responses
{
    public class AuthUserResponse : BaseResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public UserModel ExchangerUser { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
