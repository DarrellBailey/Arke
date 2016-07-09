using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arke;

namespace ArkeTests.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ArkeTcpClient client = new ArkeTcpClient();

            client.Connect("localhost", 4444);

            ArkeMessage message = new ArkeMessage("Hello Server!");

            client.Send(message);

            Console.ReadLine();
        }
    }
}
