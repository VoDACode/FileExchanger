using FileExchanger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;

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
            {
                if(root[path[i]] is JArray)
                {
                    var item = ((JArray)root[path[i]])[int.Parse(path[i + 1])];
                    setVal((JObject)item);
                    return Ok();
                }
                else
                {
                    root = (JObject)root[path[i]];
                }
            }
            setVal(root);
            return Ok();
            void setVal(JObject p)
            {
                int num;
                bool b;
                if (int.TryParse(v, out num))
                {
                    p[path.Last()] = num;
                }
                else if (bool.TryParse(v, out b))
                {
                    p[path.Last()] = b;
                }
                else
                {
                    p[path.Last()] = v;
                }
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteParameter(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                return BadRequest();
            var path = p.Split('.');
            var root = Config.Instance.ConfigFile;

            for (int i = 0; i < path.Length - 1; i++)
            {
                if (root[path[i]] is JArray)
                {
                    var array = (JArray)root[path[i]];
                    var item = array[int.Parse(path[i + 1])];
                    array.Remove(item);
                    Config.Rewrite();
                    return Ok();
                }
                else
                {
                    root = (JObject)root[path[i]];
                }
            }
            return Ok();
        }

        [HttpPost("add")]
        public IActionResult AddParameter(string p, string value)
        {
            if(string.IsNullOrWhiteSpace(p) || string.IsNullOrWhiteSpace(value))
                return BadRequest();
            JObject json = (JObject)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(Convert.FromBase64String(value)));
            var path = p.Split('.');
            var root = Config.Instance.ConfigFile;
            JArray array = default;
            for (int i = 0; i < path.Length; i++)
            {
                if (root[path[i]] is JArray)
                {
                    array = root[path[i]] as JArray;
                    break;
                }
                else
                {
                    root = (JObject)root[path[i]];
                }
            }
            if(array == default)
                return BadRequest();
            array.Add(json);       
            Config.Rewrite();
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
