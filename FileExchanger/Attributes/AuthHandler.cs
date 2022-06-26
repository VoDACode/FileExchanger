using Core;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FileExchanger.Attributes
{
    public class AuthHandler : AuthorizationHandler<AuthRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
        {
            if ((requirement.Service == DefaultService.FileExchanger && Config.Instance.Services.FileExchanger.UseAuth) ||
                (requirement.Service == DefaultService.FileStorage && Config.Instance.Services.FileStorage.UseAuth))
            {
                var claim = (context.User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                {
                    context.Fail();
                }
                else
                {
                    int id = -1;
                    if(int.TryParse(claim.Value, out id))
                    {
                        using (DbApp db = new DbApp(Config.Instance.DbConnect))
                        {
                            if (db.AuthClients.Any(p => p.Id == id))
                                context.Succeed(requirement);
                            else
                                context.Fail();
                        }
                    }
                    else
                    {
                        context.Fail();
                    }

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
