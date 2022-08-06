using MessagePack;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using websocketserver.ClientSocket;

namespace websocketclient.ClientSocket
{
    public class Client
    {
        private ClientWebSocket socket=new ClientWebSocket();

        private static CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationToken token = cts.Token;

        public async Task ConnectToTockenAsync(Uri uri)
        {
            await socket.ConnectAsync(uri, token);
        }

        public async Task SendObjAsync(object request)
        {
            var serialized = MessagePackSerializer.Serialize(request);
            await socket.SendAsync(serialized, WebSocketMessageType.Binary, true, token);
        }

        public async Task SendAsJsonAsync(RequestObject obj)
        {
            var objson = MessagePackSerializer.SerializeToJson(obj);
            var objconv = MessagePackSerializer.ConvertFromJson(objson);
            await socket.SendAsync(new ArraySegment<byte>(objconv), WebSocketMessageType.Binary, true, token);
        }



        public async Task StartReceivingAsync()
        {
            Console.WriteLine("Started receiving messages");
            var buf = new byte[1024];
            while (!token.IsCancellationRequested)
            {
                try
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buf), token);

                    var resultString = Encoding.UTF8.GetString(buf);
                    Console.WriteLine($"Server: " + resultString);
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine("Task was canseled");
                }
            }
        }
    }
}