using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arke;
using System.Threading;

namespace ArkeTests.Client
{
    public class Program
    {
        static ArkeTcpClient client = new ArkeTcpClient();

        public static void Main(string[] args)
        {
            CanRegisterMessageReceived();

            CanRegisterCallbacks();

            CanConnect();

            CanSendSimpleMessage();

            CanSendChannelMessage();

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
    }
}
