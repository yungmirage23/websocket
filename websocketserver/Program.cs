using System;
using System.Net;
using System.Threading.Tasks;
using websocketserver.ServerSocket;

namespace websocketserver
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            string serverUri = "http://localhost:80/ws/";
            var server = new Server();
            server.Start(serverUri);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

    }
}