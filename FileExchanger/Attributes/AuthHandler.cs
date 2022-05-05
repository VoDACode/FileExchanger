using Core;
using FileExchanger.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Attributes
{
    public class AuthHandler : AuthorizationHandler<AuthRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
        {
            if ((requirement.Service == Configs.DefaultService.FileExchanger && Config.Instance.Services.FileExchanger.UseAuth) ||
                (requirement.Service == Configs.DefaultService.FileStorage && Config.Instance.Services.FileStorage.UseAuth))
            {
                var a = context.Resource;
                using (DbApp db = new DbApp(Config.Instance.DbConnect))
                {
                    if (db.AuthClients.Any(p => p.Email == context.User.Identity.Name))
                        context.Succeed(requirement);
                    else
                        context.Fail();
                }
            }
            else
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
