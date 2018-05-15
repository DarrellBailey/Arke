using Arke.E2E.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.Net.E2E
{
    public class ArkeNetE2E : EndToEnd
    {
        private ArkeTcpClient client;

        private ArkeTcpServer server;

        private ManualResetEvent reset;

        public override string TestName => "Arke.Net";

        public override async Task<TestResult> Run()
        {
            LogDebug("Beginning ArkeNet End To End Testing");

            try
            {
                Initialize();
                await Connect();

                bool result = true;

                result = await SimpleMessage();
                if (!result) return TestResult.Failure;

                result = await ObjectMessage();
                if (!result) return TestResult.Failure;
            }
            catch (Exception e)
            {
                LogError(e.Message);

                return TestResult.Failure;
            }

            return TestResult.Success;
        }

        private void Initialize()
        {
            reset = new ManualResetEvent(false);

            server = new ArkeTcpServer(4444);

            client = new ArkeTcpClient();
        }

        private async Task Connect()
        {
            server.StartListening();

            await client.ConnectAsync("localhost", 4444);
        }

        private async Task<bool> SimpleMessage()
        {
            LogDebug("Testing: Simple string messaging between client and server");

            reset.Reset();

            server.RegisterChannelCallback(10, (message, connection) =>
            {
                if (message.GetContentAsString() == "Hello Server!")
                {
                    reset.Set();
                }
                else // otherwise dont set the reset, which will fail, and fail the test
                {
                    LogError("Error Sending Simple Message!");
                }
            });

            await client.SendAsync(new ArkeMessage("Hello Server!", 10));

            return reset.WaitOne(1000);
        }

        private async Task<bool> ObjectMessage()
        {
            LogDebug("Testing: Serialized Object messaging between client and server");

            Dictionary<string, string> testObj = new Dictionary<string, string>();
            testObj.Add("testString", "Hello World");

            reset.Reset();

            server.RegisterChannelCallback(11, (message, connection) =>
            {
                var obj = message.GetContent<Dictionary<string, string>>();
                if (obj["testString"] == "Hello World")
                {
                    reset.Set();
                }
                else // otherwise dont set the reset, which will fail, and fail the test
                {
                    LogError("Error Sending Object Message!");
                }
            });

            await client.SendAsync(new ArkeMessage(testObj , 11));

            return reset.WaitOne(1000);
        }

        public override void Dispose()
        {
            server.StopListening();
            client.Dispose();
        }
    }
}
