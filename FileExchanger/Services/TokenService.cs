using Core;
using Core.Helpers;
using Core.Models;
using FileExchanger.Helpers;
using FileExchanger.Interfaces;
using FileExchanger.Requests;
using FileExchanger.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Services
{
    public class TokenService : ITokenService
    {
        private readonly DbApp db;
        public TokenService(DbApp db)
        {
            this.db = db;
        }

        public async Task<Tuple<string, string>> GenerateTokensAsync(int userId)
        {
            var accessToken = await JwtHelper.CreateToken(userId);
            var refreshToken = await JwtHelper.GenerateRefreshToken();

            var userRecord = await db.AuthClients.Include(o => o.RefreshTokens).FirstOrDefaultAsync(e => e.Id == userId);

            if (userRecord == null)
            {
                return null;
            }

            var salt = PasswordHelper.GetSecureSalt;

            var refreshTokenHashed = PasswordHelper.GetHash(refreshToken, salt);

            if (userRecord.RefreshTokens != null && userRecord.RefreshTokens.Any())
            {
                await RemoveRefreshTokenAsync(userRecord);
            }

            userRecord.RefreshTokens?.Add(new RefreshTokenModel
            {
                ExpiryDate = DateTime.Now.AddDays(14),
                Ts = DateTime.Now,
                UserId = userId,
                TokenHash = refreshTokenHashed,
                TokenSalt = Convert.ToBase64String(salt)

            });

            await db.SaveChangesAsync();

            var token = new Tuple<string, string>(accessToken, refreshToken);

            return token;
        }

        public async Task<bool> RemoveRefreshTokenAsync(AuthClientModel user)
        {
            var userRecord = await db.AuthClients.Include(o => o.RefreshTokens).FirstOrDefaultAsync(e => e.Id == user.Id);

            if (userRecord == null)
                return false;

            if (userRecord.RefreshTokens != null && userRecord.RefreshTokens.Any())
            {
                var currentRefreshToken = userRecord.RefreshTokens.First();
                db.RefreshTokens.Remove(currentRefreshToken);
            }

            return false;
        }

        public async Task<RefreshTokenResponse> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(o => o.UserId == refreshTokenRequest.UserId);

            var response = new RefreshTokenResponse();
            if (refreshToken == null)
            {
                response.Success = false;
                response.Error = "Invalid session.";
                response.ErrorCode = "RT01";
                return response;
            }

            var refreshTokenToValidateHash = PasswordHelper.GetHash(refreshTokenRequest.RefreshToken, Convert.FromBase64String(refreshToken.TokenSalt));

            if (refreshToken.TokenHash != refreshTokenToValidateHash)
            {
                response.Success = false;
                response.Error = "Invalid refresh token";
                response.ErrorCode = "RT02";
                return response;
            }

            if (refreshToken.ExpiryDate < DateTime.Now)
            {
                response.Success = false;
                response.Error = "Refresh token has expired";
                response.ErrorCode = "RT03";
                return response;
            }

            response.Success = true;
            response.UserId = refreshToken.UserId;

            return response;
        }
    }
}
