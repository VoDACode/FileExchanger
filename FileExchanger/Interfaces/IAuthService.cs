using FileExchanger.Requests;
using FileExchanger.Responses;
using System.Threading.Tasks;

namespace FileExchanger.Interfaces
{
    public interface IAuthService
    {
        Task<AuthUserResponse> GetAuthInfoAsync(int userId);
        Task<TokenResponse> LoginAsync(LoginRequest loginRequest);
        Task<LogoutResponse> LogoutAsync(int userId);
        Task<RegistrationResponse> RegistrationAsync(RegistrationRequest registrationRequest);
    }
}
