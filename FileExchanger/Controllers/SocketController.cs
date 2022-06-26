using Core;
using Core.Models;
using FileExchanger.Helpers;
using FileExchanger.Models;
using FileExchanger.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FileExchanger.Controllers
{
    [Route("ws")]
    [ApiController]
    public class SocketController : BaseController
    {
        private IMemoryCache cache;
        public SocketController(DbApp db, IMemoryCache memoryCache) : base(db)
        {
            cache = memoryCache;
        }

        [HttpGet("auth")]
        public async Task WaitAuthInTelegram(string c)
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            db.AuthClients.ToList();
            var user = db.TelegramUsers.SingleOrDefault(p => p.AuthKey == c && p.IsAuth);
            if (string.IsNullOrWhiteSpace(c) || user == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
            WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            TelegramBotService.Instance.OnAuth += OnAuth;
            while (true)
            {
                var status = "";
                if (!cache.TryGetValue<string>($"{c}_tg_auth", out status))
                {
                    await socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { type = "error", data = "TIMEOUT" })), WebSocketMessageType.Text, true, CancellationToken.None);
                    break;
                }
                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (status == "-1")
                        await socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { type = "fail", data = "" })), WebSocketMessageType.Text, true, CancellationToken.None);
                    else
                        await socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { type = "ok", data = status })), WebSocketMessageType.Text, true, CancellationToken.None);
                    break;
                }
                Thread.Sleep(250);
            }
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            TelegramBotService.Instance.OnAuth -= OnAuth;
            async void OnAuth(ITelegramBotClient botClient, Update update, bool isAuth, int userId)
            {
                if (userId != user.Id)
                    return;
                var token = await JwtHelper.CreateToken(user.AuthClient.Id);
                cache.Set($"{c}_tg_auth", isAuth ? token : "-1");
            }
        }
    }
}
