using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace FileExchanger.Configs
{
    public class AuthConfig
    {
        public string Key => (string)Config.Instance.ConfigFile["Auth"]["KEY"];
        public string Issuer => (string)Config.Instance.ConfigFile["Auth"]["ISSUER"];
        public string Audience => (string)Config.Instance.ConfigFile["Auth"]["AUDIENCE"];
        public TimeSpan LifeTime => TimeSpan.FromHours((int)Config.Instance.ConfigFile["Auth"]["LIFE_TIME"]);
        public string CookiesName => "auth_token";
        public SymmetricSecurityKey GetSymmetricSecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}
