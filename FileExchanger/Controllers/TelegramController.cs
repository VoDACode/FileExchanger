using FileExchanger.Models;
using FileExchanger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileExchanger.Controllers
{
    [Authorize]
    [Route("api/telegram")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        private DbApp db;
        private IMemoryCache cache;
        AuthClientModel authClient => db.AuthClients.SingleOrDefault(p => p.Email == User.Identity.Name);
        public TelegramController(DbApp db, IMemoryCache memoryCache)
        {
            this.db = db;
            cache = memoryCache;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(string e)
        {
            if (string.IsNullOrWhiteSpace(e))
                return BadRequest();
            if (!Config.Instance.Security.Telegram.Enable)
                return BadRequest();
            var authUser = db.AuthClients.SingleOrDefault(p => p.Email == e);
            if (authUser == null)
                return BadRequest();
            var tgUser = db.TelegramUsers.SingleOrDefault(p => p.AuthClient == authUser);
            if(tgUser == null)
                return NotFound();
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("IP", Request.Host.Host);
            //Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0
            string agent = Request.Headers["User-Agent"];
            var os = "";
            var browser = agent.Split(' ').Last();
            {
                int startIndex = agent.IndexOf('(');
                int endIndex = agent.IndexOf(')');
                string[] tmp = agent.Substring(startIndex, endIndex).Split("; ");
                for(int i = 0; i < 2; i++)
                    os += tmp[i] + "; ";
            }
            info.Add("OS", os);
            info.Add("Browser", browser);
            tgUser.AuthKey = "".RandomString(128);
            TelegramBotService.Instance.SendConfirmLogin(tgUser, info);
            db.SaveChanges();
            cache.Set($"{tgUser.AuthKey}_tg_auth", "", DateTimeOffset.Now.AddHours(1));
            return Ok(tgUser.AuthKey);
        }

        [HttpGet("enable")]
        public IActionResult IsConnectTelegram()
        {
            return Ok(Config.Instance.Security.Telegram.Enable &&
                db.TelegramUsers.Any(p => p.IsAuth && p.AuthClient == authClient));
        }

        [HttpDelete("delete")]
        public IActionResult DeleteTelegram()
        {
            var tg = db.TelegramUsers.SingleOrDefault(p => p.AuthClient == authClient);
            if (tg == null)
                return NotFound();
            db.TelegramUsers.Remove(tg);
            db.SaveChanges();
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("info")]
        public IActionResult GetBotUrl()
        {
            return Ok($"https://t.me/{TelegramBotService.Instance.Username}");
        }
    }
}
