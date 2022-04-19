using FileExchanger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FileExchanger.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("api/config")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private DbApp db;
        private AuthClientModel? authClient => db.AuthClients.SingleOrDefault(p => p.Email == User.Identity.Name);

        public ConfigController(DbApp db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetConfig()
        {
            return Ok(Config.Instance.ToString());
        }

        [HttpPost("set")]
        public IActionResult SetParameter(string p, string v)
        {
            if (string.IsNullOrWhiteSpace(p) || string.IsNullOrWhiteSpace(v))
                return BadRequest();
            var path = p.Split('.');
            var root = Config.Instance.ConfigFile;

            for (int i = 0; i < path.Length - 1; i++)
                root = (Newtonsoft.Json.Linq.JObject)root[path[i]];
            int num;
            bool b;
            if(int.TryParse(v, out num))
            {
                root[path.Last()] = num;
            }else if(bool.TryParse(v, out b))
            {
                root[path.Last()] = b;
            }
            else
            {
                root[path.Last()] = v;
            }
            return Ok();
        }

        [HttpPost("reload")]
        public IActionResult ReloadConfig()
        {
            Config.Rewrite();
            return Ok();
        }
    }
}
