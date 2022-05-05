using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Core.Helpers
{
    public static class WebSocketHelper
    {
        public static async Task SendAsync(this WebSocket socket, string data, WebSocketMessageType type, bool endOfMessage, CancellationToken token)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            await socket.SendAsync(buffer, type, endOfMessage, token);
            
        }
        public static async Task<string> ReceiveStringAsync(this WebSocket socket, CancellationToken token)
        {
            byte[] buffer = new byte[2048];
            var res = await socket.ReceiveAsync(buffer, token);
            var resultBuffer = new byte[res.Count];
            Array.Copy(buffer, 0, resultBuffer, 0, resultBuffer.Length);
            return Encoding.UTF8.GetString(resultBuffer);
        }
        public static string ReceiveString(this WebSocket socket, CancellationToken token, out WebSocketReceiveResult result)
        {
            byte[] buffer = new byte[2048];
            result = socket.ReceiveAsync(buffer, token).Result;
            var resultBuffer = new byte[result.Count];
            Array.Copy(buffer, 0, resultBuffer, 0, resultBuffer.Length);
            return Encoding.UTF8.GetString(resultBuffer);
        }
        public static async Task<byte[]> ReceiveAsync(this WebSocket socket, CancellationToken token, int bufferSize = 2048)
        {
            byte[] buffer = new byte[bufferSize];
            await socket.ReceiveAsync(buffer, token);
            return buffer;
        }
        public static byte[] Receive(this WebSocket socket, CancellationToken token, out WebSocketReceiveResult result, int bufferSize = 2048)
        {
            byte[] buffer = new byte[bufferSize];
            result = socket.ReceiveAsync(buffer, token).Result;
            return buffer;
        }
    }
}
