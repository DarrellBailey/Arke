using Arke.E2E.Common;
using System;

namespace Arke.Net.E2E.Server
{
    public class ArkeTcpServerE2E : EndToEnd
    {
        public override TestResult Run()
        {
            throw new NotImplementedException();
        }

        static ArkeTcpServer server = new ArkeTcpServer(4444);

        public static void Main(string[] args)
        {
            CanRegisterConnectionEvent();

            CanRegisterMessageReceivedEvent();

            CanRegisterChannelMessageReceivedEvent();

            CanRegisterRequestResponseCallback();

            CanRegisterRequestResponseChannelCallback();

            CanStartListening();

            Console.ReadLine();
        }

        public static void CanStartListening()
        {
            server.StartListening();

            Console.WriteLine("Arke Listening On Port 4444");
        }

        public static void CanRegisterConnectionEvent()
        {
            server.ConnectionReceived += connection =>
            {
                Console.WriteLine("Connection Established For Client With Id: " + connection.Id);

                connection.Send(new ArkeMessage("Server: Connection Established"));
            };
        }

        public static void CanRegisterMessageReceivedEvent()
        {
            server.MessageReceived += (message, connection) =>
            {
                Console.WriteLine("Message '" + message.GetContentAsString() + "' With Content Type: " + message.ContentType + " Received From: " + connection.Id);

                connection.Send(new ArkeMessage("Server: Message Received By Generic Handler"));
            };
        }

        public static void CanRegisterChannelMessageReceivedEvent()
        {
            server.RegisterChannelCallback(1, (message, connection) =>
            {
                Console.WriteLine(" Channel Message '" + message.GetContentAsString() + "' With Content Type: " + message.ContentType + " Received From: " + connection.Id);

                connection.Send(new ArkeMessage("Server: Message Received By Channel 1 Handler"));
            });
        }

        public static void CanRegisterRequestResponseCallback()
        {
            server.RegisterRequestResponseCallback(async (message, connection) =>
            {
                Console.WriteLine(" Channel Message '" + message.GetContentAsString() + "' With Content Type: " + message.ContentType + " Received From: " + connection.Id);

                return new ArkeMessage("Server: Request Response Received By Generic Handler");
            });
        }

        public static void CanRegisterRequestResponseChannelCallback()
        {
            server.RegisterRequestResponseChannelCallback(1, async (message, connection) =>
            {
                Console.WriteLine(" Channel Message '" + message.GetContentAsString() + "' With Content Type: " + message.ContentType + " Received From: " + connection.Id);

                return new ArkeMessage("Server: Request Response Received By Channel 1 Handler");
            });
        }

        public static void CanDetectDisconnection()
        {
            server.Disconnected += connection =>
            {
                Console.WriteLine("Client: " + connection.Id + " disconnected.");
            };
        }
    }
}
