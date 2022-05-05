using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Core.Zip;
using Core.Models;
using Core.Helpers;
using System.Threading;
using ZipServer.Services;
using System.Linq;
using System.Net.WebSockets;
using Newtonsoft.Json;
using System.IO;
using ZipServer.Managers;

namespace ZipServer.Controllers
{
    [Route("api/zip")]
    [ApiController]
    public class ZipController : ControllerBase
    {
        [HttpGet("ws")]
        public async Task<IActionResult> get(string token)
        {
            if(token != Config.Instance.Token)
                return Unauthorized();
            if (!HttpContext.WebSockets.IsWebSocketRequest)
                return BadRequest();
            var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var zip = new ZipService();
            bool waitContent = false;
            bool setName = false;
            while (ws.State == WebSocketState.Open)
            {
                string data = await ws.ReceiveStringAsync(CancellationToken.None);
                if (data == ZipCommands.StartAddItem)
                {
                    waitContent = true;
                }
                else if (data == ZipCommands.EndAddItem)
                {
                    waitContent = false;
                }
                else if (waitContent)
                {
                    await zip.Add(JsonConvert.DeserializeObject<ZipItem>(data));
                }
                else if(data == ZipCommands.SetName)
                {
                    setName = true;
                }
                else if (setName)
                {
                    Storage.Instance.ZipStorage[zip.Key].Name = data;
                    setName = false;
                }
                else if (data == ZipCommands.Create)
                {
                    await zip.Create();
                    await ws.SendAsync(zip.Key, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if(data == ZipCommands.Pack)
                {
                    await zip.Pack();
                    await ws.SendAsync(ZipCommands.Bye, WebSocketMessageType.Text, true, CancellationToken.None);
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "End session.", CancellationToken.None);
                    break;
                }
            }
            return Ok();
        }

        [HttpGet("download/{key}")]
        public async Task<IActionResult> DownloadZip(string key)
        {
            if (!Storage.Instance.ZipStorage.ContainsKey(key))
                return NotFound();
            var zip = Storage.Instance.ZipStorage[key];
            var fileData = System.IO.File.ReadAllBytes(Path.Combine(FileManager.Instance.TempPath, $"{key}_zip", "result.zip"));
            HttpContext.Response.StatusCode = 200;
            HttpContext.Response.ContentType = "application/octet-stream";
            HttpContext.Response.ContentLength = fileData.Length;
            HttpContext.Response.Headers.Add("content-disposition", $"attachment; filename=\" {zip.Name}.zip\"; filename*=UTF-8''{zip.Name}.zip");
            await HttpContext.Response.Body.WriteAsync(fileData, 0, fileData.Length);
            if (Directory.Exists(Path.Combine(FileManager.Instance.TempPath, $"{key}_zip")))
            {
                Directory.Delete(Path.Combine(FileManager.Instance.TempPath, $"{key}_zip"), true);
                Storage.Instance.ZipStorage.Remove(key);
            }
            return Ok();
        }
    }
}
