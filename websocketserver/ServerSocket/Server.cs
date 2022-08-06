using MessagePack;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace websocketserver.ServerSocket
{
    internal class Server
    {
        private int count = 0;
        public async void Start(string listenerPrefix)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(listenerPrefix);
            listener.Start();
            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext listenerContext = await listener.GetContextAsync();
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    ProcessRequest(listenerContext);
                }
                else
                {
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
                }
            }
        }
        private async void ProcessRequest(HttpListenerContext listenerContext)
        {
            WebSocketContext webSocketContext = null;
            try
            {
                webSocketContext = await listenerContext.AcceptWebSocketAsync(subProtocol: null);
                Interlocked.Increment(ref count);
                Console.WriteLine("Processed: {0}", count);
            }
            catch (Exception e)
            {
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Console.WriteLine("Exception: {0}", e);
                return;
            }

            WebSocket webSocket = webSocketContext.WebSocket;

            try
            {
                byte[] receiveBuffer = new byte[1024];
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    else if (receiveResult.MessageType == WebSocketMessageType.Binary)
                    {
                        var jsontxt = MessagePackSerializer.ConvertToJson(receiveBuffer);
                        var objres = MessagePackSerializer.Deserialize<RequestObject>(receiveBuffer);
                        Console.WriteLine($"Client {webSocket.GetHashCode()}: id:{objres.Id}\tcomment : {objres.Comment}\timage bytes length : {objres.Image.Length}");
                        await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"hello client, thanks for sending object id:{objres.Id}")), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else if (receiveResult.MessageType == WebSocketMessageType.Text)
                    {

                        await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"hello client, thanks for sending object id:")), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                        await webSocket.SendAsync(new ArraySegment<byte>(receiveBuffer), WebSocketMessageType.Binary, true, CancellationToken.None);
                    }  
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
            }
        }
    }
}