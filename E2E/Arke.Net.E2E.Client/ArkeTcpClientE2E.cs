using Arke.E2E.Common;
using System;

namespace Arke.Net.E2E.Client
{
    public class ArkeTcpClientE2E : EndToEnd
    {
        public override TestResult Run()
        {
            throw new NotImplementedException();
        }

        static ArkeTcpClient client = new ArkeTcpClient();

        public static void Main(string[] args)
        {
            CanRegisterMessageReceived();

            CanRegisterCallbacks();

            CanConnect();

            CanSendSimpleMessage();

            CanSendChannelMessage();

            CanSendRequestResponseMessage();

            CanSendRequestResponseChannelMessage();

            CanSeeDisconnect();

            CanDisconnectByDispose();

            Console.ReadLine();
        }

        public static void CanConnect()
        {
            client.Connect("localhost", 4444);
        }

        public static void CanSendSimpleMessage()
        {
            ArkeMessage message = new ArkeMessage("Hello Server!");

            client.Send(message);
        }

        public static void CanSendChannelMessage()
        {
            ArkeMessage message = new ArkeMessage("Hello Server!", 1);

            client.Send(message);
        }

        public static void CanRegisterMessageReceived()
        {
            client.MessageReceived += (message, client) =>
            {
                Console.WriteLine(message.GetContentAsString());
            };
        }

        public static void CanRegisterCallbacks()
        {
            client.RegisterChannelCallback(1, (message, client) =>
            {
                Console.WriteLine(message.GetContentAsString());
            });
        }

        public static void CanSendRequestResponseMessage()
        {
            ArkeMessage message = new ArkeMessage("Hello Server! Give Me A Response");

            ArkeMessage response = client.SendRequest(message);

            Console.WriteLine(response.GetContentAsString());
        }

        public static void CanSendRequestResponseChannelMessage()
        {
            ArkeMessage message = new ArkeMessage("Hello Server! Give Me A Response", 1);

            ArkeMessage response = client.SendRequest(message);

            Console.WriteLine(response.GetContentAsString());
        }

        public static void CanSeeDisconnect()
        {
            client.Disconnected += client =>
            {
                Console.WriteLine("Client Disconnected");
            };
        }

        public static void CanDisconnectByDispose()
        {
            client.Dispose();
        }
    }
}
