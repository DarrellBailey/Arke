﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arke;

namespace ArkeTests.Server
{
    public class Program
    {
        static ArkeTcpServer server = new ArkeTcpServer(4444);

        public static void Main(string[] args)
        {
            CanRegisterConnectionEvent();

            CanRegisterMessageReceivedEvent();

            CanRegisterChannelMessageReceivedEvent();

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
    }
}
