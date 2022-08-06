using System;

using System.Threading.Tasks;
using websocketclient.ClientSocket;
using websocketserver.ClientSocket;

namespace websocketclient
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            string clientUri = "ws://localhost:80/ws/";
            var client = new Client();


            await client.ConnectToTockenAsync(new Uri(clientUri));
            Console.WriteLine("Ws connected");

            client.StartReceivingAsync();

            var targetObject = new RequestObject
            {
                Id = 123,
                Image = new byte[] { 21, 21, 21, 21 },
                Comment = "This is test."
            };
            targetObject.Tags.Add("Sample");
            targetObject.Tags.Add("Example");

            await client.SendObjAsync(targetObject);
            await client.SendAsJsonAsync(targetObject);

            Console.ReadLine();
        }
    }
}