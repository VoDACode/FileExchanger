using Core.Zip;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Net;
using System.Net.Http;
using System;
using System.Threading;
using Core.Helpers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FileExchanger.Services
{
    public class ZipService : ZipContext
    {
        private ClientWebSocket socket { get; set; }

        public override async Task<ZipContext> Create()
        {
            socket = new ClientWebSocket();
            var url = new Uri($"wss://{Config.Instance.Services.ZipServer.Host}:{Config.Instance.Services.ZipServer.Port}/api/zip/ws?token={Config.Instance.Services.ZipServer.Token}");
            socket.Options.RemoteCertificateValidationCallback = (s, cer, c, ssl) => true;
            await socket.ConnectAsync(url, CancellationToken.None);
            await socket.SendAsync(ZipCommands.Create, WebSocketMessageType.Text, true, CancellationToken.None);
            _key = await socket.ReceiveStringAsync(CancellationToken.None);
            return this;
        }

        public override async Task<ZipContext> AddRage(List<ZipItem> items)
        {
            foreach(ZipItem item in items)
                await Add(item);
            return this;
        }

        public override async Task<ZipContext> Add(ZipItem item)
        {
            await socket.SendAsync(ZipCommands.StartAddItem, WebSocketMessageType.Text, true, CancellationToken.None);
            await socket.SendAsync(JsonConvert.SerializeObject(item), WebSocketMessageType.Text, true, CancellationToken.None);
            await socket.SendAsync(ZipCommands.EndAddItem, WebSocketMessageType.Text, true, CancellationToken.None);
            return this;
        }

        public override async Task<ZipContext> Pack()
        {
            await socket.SendAsync(ZipCommands.Pack, WebSocketMessageType.Text, true, CancellationToken.None);
            var result = await socket.ReceiveStringAsync(CancellationToken.None);
            return this;
        }

        public async Task SetName(string name)
        {
            await socket.SendAsync(ZipCommands.SetName, WebSocketMessageType.Text, true, CancellationToken.None);
            await socket.SendAsync(name, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public override Stream? GetStream()
        {
            var h = new HttpClientHandler();
            h.ClientCertificateOptions = ClientCertificateOption.Manual;
            h.ServerCertificateCustomValidationCallback += (s, ce, ch, sslPE) => true;
            HttpClient httpClient = new HttpClient(h);
            var responce = httpClient.GetAsync($"https://{Config.Instance.Services.ZipServer.Host}:{Config.Instance.Services.ZipServer.Port}/api/zip/download/{Key}?token={Config.Instance.Services.ZipServer.Token}").Result;
            if(responce.StatusCode == HttpStatusCode.OK)
                return responce.Content.ReadAsStreamAsync().Result;
            return null;
        }
    }
}
