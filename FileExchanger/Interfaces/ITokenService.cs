using System;
using System.Threading.Tasks;
using Core.Models;
using FileExchanger.Requests;
using FileExchanger.Responses;

namespace FileExchanger.Interfaces
{
    public interface ITokenService
    {
        Task<Tuple<string, string>> GenerateTokensAsync(int userId);
        Task<RefreshTokenResponse> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        Task<bool> RemoveRefreshTokenAsync(AuthClientModel user);
    }
}
