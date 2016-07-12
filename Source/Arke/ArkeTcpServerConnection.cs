using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arke
{
    /// <summary>
    /// An instance of a single connection between an Arke Client and Arke Server
    /// </summary>
    public class ArkeTcpServerConnection
    {
        private ArkeTcpServerConnection() { }

        private ArkeTcpServer _server;

        private Dictionary<int, List<ConnectionMessageReceivedHandler>> _channelHandlers = new Dictionary<int, List<ConnectionMessageReceivedHandler>>();

        /// <summary>
        /// The generated Id for this connection.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The underlying client for this Server Connection.
        /// </summary>
        public ArkeTcpClient Client { get; private set; }

        internal ArkeTcpServerConnection(Guid id, ArkeTcpClient client, ArkeTcpServer server)
        {
            Id = id;

            Client = client;

            _server = server;

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
        public async Task SendAsync(ArkeMessage message)
        {
            await Client.SendAsync(message);
        }

        /// <summary>
        /// Send A Message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(ArkeMessage message)
        {
            Client.SendAsync(message).Wait();
        }

        /// <summary>
        /// Register a callback for a specific channel. You can register more than one callback on a single channel.
        /// </summary>
        /// <param name="channel">The channel to register to.</param>
        /// <param name="callback">The callback to register.</param>
        public void RegisterChannelCallback(int channel, ConnectionMessageReceivedHandler callback)
        {
            if (!_channelHandlers.ContainsKey(channel))
            {
                _channelHandlers.Add(channel, new List<ConnectionMessageReceivedHandler>());
            }

            List<ConnectionMessageReceivedHandler> handlers = _channelHandlers[channel];

            if (!handlers.Contains(callback))
            {
                handlers.Add(callback);
            }
        }

        private void OnMessageReceived(ArkeMessage message, ArkeTcpClient client)
        {
            List<ConnectionMessageReceivedHandler> handlers;

            bool hasHandlers = _channelHandlers.TryGetValue(message.Channel, out handlers);

            if (hasHandlers)
            {
                handlers.ForEach(callback => callback(message, this));
            }

            MessageReceived?.Invoke(message, this);
        }

        #region Events

        /// <summary>
        /// Triggered when this connection receives a message.
        /// </summary>
        public event ConnectionMessageReceivedHandler MessageReceived;

        #endregion
    }

    #region Delegates

    /// <summary>
    /// The message received delegate for an ArkeTcpConnection MessageReceived event.
    /// </summary>
    /// <param name="message">The message that was received.</param>
    /// <param name="connection">The connection that received the message.</param>
    public delegate void ConnectionMessageReceivedHandler(ArkeMessage message, ArkeTcpServerConnection connection);

    #endregion
}
