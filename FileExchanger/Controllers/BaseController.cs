using Core;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace FileExchanger.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected DbApp db;
        protected int UserID
        {
            get
            {
                int result = -1;
                var claim = (User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null && int.TryParse(claim.Value, out result))
                    return result;
                return 2;
            }
        }
        protected AuthClientModel? AuthClient => db.AuthClients.SingleOrDefault(p => p.Id == UserID);
        public BaseController(DbApp db)
        {
            this.db = db;
        }
        public BaseController() { }
    }
}
