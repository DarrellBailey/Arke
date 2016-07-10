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

        protected Dictionary<int, List<ConnectionMessageReceivedHandler>> ChannelHandlers = new Dictionary<int, List<ConnectionMessageReceivedHandler>>();

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

        /// <summary>
        /// Register a callback for a specific channel. You can register more than one callback on a single channel.
        /// </summary>
        /// <param name="channel">The channel to register to.</param>
        /// <param name="callback">The callback to register.</param>
        public void RegisterChannelCallback(int channel, ConnectionMessageReceivedHandler callback)
        {
            if (!ChannelHandlers.ContainsKey(channel))
            {
                ChannelHandlers.Add(channel, new List<ConnectionMessageReceivedHandler>());
            }

            List<ConnectionMessageReceivedHandler> handlers = ChannelHandlers[channel];

            if (!handlers.Contains(callback))
            {
                handlers.Add(callback);
            }
        }

        protected void OnMessageReceived(ArkeMessage message)
        {
            MessageReceived?.Invoke(message, this);

            List<ConnectionMessageReceivedHandler> handlers;

            bool hasHandlers = ChannelHandlers.TryGetValue(message.Channel, out handlers);
            
            if(hasHandlers)
            {
                handlers.ForEach(callback => callback(message, this));
            }
        }

        #region Events

        public event ConnectionMessageReceivedHandler MessageReceived;

        #endregion
    }

    #region Delegates

    public delegate void ConnectionMessageReceivedHandler(ArkeMessage message, ArkeTcpServerConnection connection);

    #endregion
}
