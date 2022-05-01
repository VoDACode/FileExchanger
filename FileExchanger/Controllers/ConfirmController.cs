﻿using FileExchanger.Helpers;
using FileExchanger.Models;
using FileExchanger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using Telegram.Bot;

namespace FileExchanger.Controllers
{
    [Route("api/c")]
    [ApiController]
    public class ConfirmController : ControllerBase
    {
        DbApp db;
        IMemoryCache cache;
        AuthClientModel authClient => db.AuthClients.SingleOrDefault(p => p.Email == User.Identity.Name);
        public ConfirmController(DbApp db, IMemoryCache memoryCache)
        {
            this.db = db;
            cache = memoryCache;
        }

        [HttpGet("e/{key}")]
        public IActionResult ConfirmEmail(string key)
        {
            AuthClientModel authClient = new AuthClientModel();
            if (!cache.TryGetValue($"EMAIL_CONFIRM_{key}", out authClient))
                return Redirect("/");

            cache.Remove($"EMAIL_CONFIRM_{key}");
            db.AuthClients.Add(authClient);
            db.Directory.Add(new DirectoryModel()
            {
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Name = "/",
                Key = "root",
                Owner = authClient
            });
            db.SaveChanges();
            Response.Cookies.Append(Config.Instance.Auth.CookiesName, JwtHelper.CreateToken(authClient.Email, authClient.Password));
            return Redirect("/");
        }

        [Authorize]
        [HttpGet("telegram")]
        public IActionResult ConfirmTelegram(string code)
        {
            if (string.IsNullOrEmpty(code))
                return NotFound();
            var tgClient = db.TelegramUsers.SingleOrDefault(p => p.AuthKey == code && !p.IsAuth);
            if (tgClient == null)
                return NotFound();
            tgClient.AuthClient = authClient;
            tgClient.IsAuth = true;
            tgClient.AuthKey = null;
            db.SaveChanges();
            TelegramBotService.Instance.Bot.SendTextMessageAsync(tgClient.ChatId, "Done!");
            return Ok();
        }
    }
}