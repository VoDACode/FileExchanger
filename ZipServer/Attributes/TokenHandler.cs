using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ZipServer.Attributes
{
    public class TokenHandler : AuthorizationHandler<TokenRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenRequirement requirement)
        {
            var http = context.Resource as DefaultHttpContext;
            if (http.Request.Query["token"] == Config.Instance.Token)
                context.Succeed(requirement);
            else
            {
                http.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Fail();
            }
        }
    }
}
