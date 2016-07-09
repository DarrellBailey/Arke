using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arke;

namespace ArkeTests.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ArkeTcpServer server = new ArkeTcpServer(4444);

            server.ConnectionReceived += Server_ConnectionReceived;

            server.MessageReceived += Server_MessageReceived;

            server.StartListening();

            Console.WriteLine("Arke Listening On Port 4444");

            Console.ReadLine();
        }

        private static void Server_MessageReceived(ArkeMessage message, ArkeTcpServerConnection connection)
        {
            Console.WriteLine("Message '" + message.GetContentAsString() + "' With Content Type: " + message.ContentType +" Received From: " + connection.Id);
        }

        private static void Server_ConnectionReceived(ArkeTcpServerConnection connection)
        {
            Console.WriteLine("Connection Established For Client With Id: " + connection.Id);
        }
    }
}
