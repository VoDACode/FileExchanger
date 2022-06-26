using Core;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FileExchanger.Attributes
{
    public class AdminHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            var claim = (context.User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                context.Fail();
            }
            else
            {
                using (DbApp db = new DbApp(Config.Instance.DbConnect))
                {
                    var authUser = db.AuthClients.SingleOrDefault(p => p.Id == int.Parse(claim.Value));
                    if (authUser == null || !db.Admins.Any(p => p.AuthClient == authUser))
                        context.Fail();
                    else
                        context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
