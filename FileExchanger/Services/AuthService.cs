using Core;
using Core.Helpers;
using Core.Models;
using FileExchanger.Interfaces;
using FileExchanger.Requests;
using FileExchanger.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService tokenService;
        private readonly DbApp db;
        public AuthService(ITokenService tokenService, DbApp db)
        {
            this.tokenService = tokenService;
            this.db = db;
        }

        public async Task<AuthUserResponse> GetAuthInfoAsync(int userId)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Id == userId);
            if(user == null)
            {
                return new AuthUserResponse()
                {
                    Success = false,
                    Error = "User not found.",
                    ErrorCode = "AUTH_INFO01"
                };
            }
            return new AuthUserResponse()
            {
                Success = true,
                Email = user.Email,
                UserName = user.Name,
                CreationDate = user.Ts,
                ExchangerUser = user.ExchangerUser
            };
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await db.AuthClients.SingleOrDefaultAsync(p => p.Active && p.Email == loginRequest.Email);
            if( user == null)
            {
                return new TokenResponse()
                {
                    Success = false,
                    Error = "User not found.",
                    ErrorCode = "AUTH_LOGIN01"
                };
            }
            var passwordHash = PasswordHelper.GetHash(loginRequest.Password, Convert.FromBase64String(user.PasswordSalt));

            if (user.Password != passwordHash)
            {
                return new TokenResponse
                {
                    Success = false,
                    Error = "Invalid Password",
                    ErrorCode = "AUTH_LOGIN02"
                };
            }

            var token = await Task.Run(() => tokenService.GenerateTokensAsync(user.Id));

            return new TokenResponse
            {
                Success = true,
                AccessToken = token.Item1,
                RefreshToken = token.Item2,
                UserId = user.Id,
                Username = user.Name,
            };
        }

        public async Task<LogoutResponse> LogoutAsync(int userId)
        {
            var refreshToken = await db.RefreshTokens.SingleOrDefaultAsync(o => o.UserId == userId);

            if (refreshToken == null)
                return new LogoutResponse { Success = true };

            db.RefreshTokens.Remove(refreshToken);

            var saveResponse = await db.SaveChangesAsync();

            if (saveResponse >= 0)
                return new LogoutResponse { Success = true };

            return new LogoutResponse { Success = false, Error = "Unable to logout", ErrorCode = "AUTH_LOGOUT01" };
        }

        public async Task<RegistrationResponse> RegistrationAsync(RegistrationRequest registrationRequest)
        {
            var existingUser = await db.AuthClients.SingleOrDefaultAsync(user => user.Email == registrationRequest.Email);

            if (existingUser != null)
            {
                return new RegistrationResponse
                {
                    Success = false,
                    Error = "User already exists with the same email",
                    ErrorCode = "R01"
                };
            }

            if (registrationRequest.Password != registrationRequest.ConfirmPassword)
            {
                return new RegistrationResponse
                {
                    Success = false,
                    Error = "Password and confirm password do not match",
                    ErrorCode = "R02"
                };
            }

            if (registrationRequest.Password.Length <= 7)
            {
                return new RegistrationResponse
                {
                    Success = false,
                    Error = "Password is weak",
                    ErrorCode = "R03"
                };
            }

            if (registrationRequest.Username.Length < 4)
            {
                return new RegistrationResponse
                {
                    Success = false,
                    Error = "Username is weak",
                    ErrorCode = "R04"
                };
            }

            var salt = PasswordHelper.GetSecureSalt;
            var passwordHash = PasswordHelper.GetHash(registrationRequest.Password, salt);

            var exchangerUser = await db.Users.AddAsync(new UserModel()
            {
                RegistrationDate = DateTime.Now,
                Key = "".RandomString(128),
                LastActive = DateTime.Now,
                MaxFileCount = Config.Instance.Services.FileExchanger.MaxUploadCount,
                MaxFileSize = Config.Instance.Services.FileExchanger.MaxSaveSize,
                MaxSaveFileTime = Config.Instance.Services.FileExchanger.MaxSaveTime
            });

            var user = new AuthClientModel
            {
                Email = registrationRequest.Email,
                Password = passwordHash,
                PasswordSalt = Convert.ToBase64String(salt),
                Name = registrationRequest.Username,
                Ts = registrationRequest.Ts,
                ExchangerUser = exchangerUser.Entity,
                Active = false
            };

            await db.AuthClients.AddAsync(user);

            var saveResponse = await db.SaveChangesAsync();

            if (saveResponse >= 0)
                return new RegistrationResponse { Success = true, Email = user.Email };

            return new RegistrationResponse
            {
                Success = false,
                Error = "Unable to save the user",
                ErrorCode = "S05"
            };

        }
    }
}
