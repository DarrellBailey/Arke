using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke
{
    public class ArkeTcpServerConnection
    {
        private ArkeTcpServerConnection() { }

        protected ArkeTcpServer Server;

        public Guid Id { get; private set; }

        /// <summary>
        /// The underlying client for this Server Connection.
        /// </summary>
        public ArkeTcpClient Client { get; private set; }

        internal ArkeTcpServerConnection(Guid id, ArkeTcpClient client, ArkeTcpServer server)
        {
            Id = id;

            Client = client;

            Server = server;

            client.MessageReceived += OnMessageReceived;
        }

        internal void StartListening()
        {
            Client.StartListening();
        }

        /// <summary>
        /// Send A Message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public async Task Send(ArkeMessage message)
        {
            await Client.SendAsync(message);
        }

        protected void OnMessageReceived(ArkeMessage message)
        {
            MessageReceived?.Invoke(message, this);
        }

        #region Events

        public event ConnectionMessageReceivedHandler MessageReceived;

        #endregion
    }

    #region Delegates

    public delegate void ConnectionMessageReceivedHandler(ArkeMessage message, ArkeTcpServerConnection connection);

    #endregion
}
