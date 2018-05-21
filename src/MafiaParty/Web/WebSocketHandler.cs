using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MafiaParty.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MafiaParty.Web {
    public class WebSocketHandler : DependencyObject {

        private WebSocket _ws;

        private RealtimeContext _rtContext;

        public WebSocketHandler(ISContext serverContext, WebSocket websocket) : base(serverContext) {
            _ws = websocket;
            _rtContext = new RealtimeContext();
        }

        private async Task<string> readLineAsync() {
            var data = string.Empty;
            while (true) {
                var buf = new byte[1];
                var arraySeg = new ArraySegment<byte>(buf);
                await _ws.ReceiveAsync(arraySeg, CancellationToken.None);
                var c = (char) buf[0];
                data += c;
                if (c == '\n') return data;
            }
        }

        private Task writeLineAsync(string data) {
            lock (_ws) {
                return _ws.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(data + "\n")),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            }
        }

        public async Task eventLoopAsync() {
            _rtContext.player = new Player();
            // TODO: add player to room
        }

        public static async Task acceptWebSocketClientsAsync(HttpContext hc, Func<Task> n, ISContext sctx) {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;

            var ws = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new WebSocketHandler(sctx, ws);
            await h.eventLoopAsync();
        }

        public static void map(IApplicationBuilder app, ISContext context) {
            app.Use(async (hc, n) => await acceptWebSocketClientsAsync(hc, n, context));
        }
    }
}