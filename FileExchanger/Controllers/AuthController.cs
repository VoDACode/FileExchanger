using FileExchanger.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;
using FileExchanger.Interfaces;
using FileExchanger.Requests;
using FileExchanger.Responses;
using Microsoft.AspNetCore.Authorization;

namespace FileExchanger.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private IMemoryCache cache;
        private readonly IAuthService authService;
        public AuthController(IMemoryCache memoryCache, IAuthService authService)
        {
            cache = memoryCache;
            this.authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new TokenResponse
                {
                    Error = "Missing login data",
                    ErrorCode = "L01"
                });
            }

            var loginResponse = await authService.LoginAsync(loginRequest);

            if (!loginResponse.Success)
            {
                return Unauthorized(new
                {
                    loginResponse.ErrorCode,
                    loginResponse.Error
                });
            }
            HttpContext.Response.Cookies.Append(Config.Instance.Auth.CookiesName, loginResponse.AccessToken);
            return Ok(loginResponse);
        }

        [HttpPost("regin")]
        public async Task<IActionResult> Registration(RegistrationRequest registrationRequest)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();
                if (errors.Any())
                    return BadRequest(new TokenResponse
                    {
                        Error = $"{string.Join(",", errors)}",
                        ErrorCode = "R11"
                    });
            }

            var registrationResponse = await authService.RegistrationAsync(registrationRequest);

            if (!registrationResponse.Success)
                return UnprocessableEntity(registrationResponse);

            string confirmKey = "".RandomString(64);
            var host = HttpContext.Request.Host;
            cache.Set($"EMAIL_CONFIRM_{confirmKey}", registrationResponse.Email, DateTime.Now.AddHours(1));
            await EmailService.Send(registrationResponse.Email, "Confirm email!", $"https://{host.Host}:{host.Port}/api/c/e/{confirmKey}");

            return Ok(registrationResponse.Email);
        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            var logout = await authService.LogoutAsync(UserID);
            if (!logout.Success)
                return UnprocessableEntity(logout);
            return Ok();
        }

        [Authorize]
        [HttpGet("info")]
        public async Task<IActionResult> Info()
        {
            var userResponse = await authService.GetAuthInfoAsync(UserID);
            if (!userResponse.Success)
                return UnprocessableEntity(userResponse);
            return Ok(userResponse);
        }

        [Authorize]
        [HttpGet("check")]
        public IActionResult CheckAuth() => Ok();
    }
}
