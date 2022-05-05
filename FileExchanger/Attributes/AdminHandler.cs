using Core;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Attributes
{
    public class AdminHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            using (DbApp db = new DbApp(Config.Instance.DbConnect))
            {
                var authUser = db.AuthClients.SingleOrDefault(p => p.Email == context.User.Identity.Name);
                if (authUser == null || !db.Admins.Any(p => p.AuthClient == authUser))
                    context.Fail();
                else
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
