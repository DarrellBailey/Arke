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

        private Dictionary<int, List<ConnectionMessageReceivedHandler>> channelHandlers = new Dictionary<int, List<ConnectionMessageReceivedHandler>>();

        private Dictionary<int, ConnectionRequestResponseMessageReceivedHandler> requestResponseChannelHandlers = new Dictionary<int, ConnectionRequestResponseMessageReceivedHandler>();

        private ConnectionRequestResponseMessageReceivedHandler requestResponseMessageHandler = null;

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

            client.RegisterRequestResponseCallback(OnRequestReceived);
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
            Client.Send(message);
        }

        /// <summary>
        /// Send a request and wait for a response from the server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The response from the server.</returns>
        public ArkeMessage SendRequest(ArkeMessage message)
        {
            return Client.SendRequest(message);
        }

        /// <summary>
        /// Send a request and wait for a response from the server.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>The response from the server.</returns>
        public async Task<ArkeMessage> SendRequestAsync(ArkeMessage message)
        {
            return await Client.SendRequestAsync(message);
        }

        /// <summary>
        /// Register a callback for a specific channel. You can register more than one callback on a single channel.
        /// </summary>
        /// <param name="channel">The channel to register to.</param>
        /// <param name="callback">The callback to register.</param>
        public void RegisterChannelCallback(int channel, ConnectionMessageReceivedHandler callback)
        {
            if (!channelHandlers.ContainsKey(channel))
            {
                channelHandlers.Add(channel, new List<ConnectionMessageReceivedHandler>());
            }

            List<ConnectionMessageReceivedHandler> handlers = channelHandlers[channel];

            if (!handlers.Contains(callback))
            {
                handlers.Add(callback);
            }
        }

        /// <summary>
        /// Register a request response callback for a specific channel.
        /// </summary>
        /// <param name="channel">The channel to register to.</param>
        /// <param name="callback">The callback to register.</param>
        /// <remarks> Only one callback can be registered at a time. If more that one registration is attempted, the previous registration will be overwritten.</remarks>
        public void RegisterRequestResponseChannelCallback(int channel, ConnectionRequestResponseMessageReceivedHandler callback)
        {
            if (requestResponseChannelHandlers.ContainsKey(channel))
            {
                requestResponseChannelHandlers.Remove(channel);
            }

            requestResponseChannelHandlers.Add(channel, callback);
        }

        /// <summary>
        /// Register a global request response callback.
        /// </summary>
        /// <param name="callback">The callback to register.</param>
        public void RegisterRequestResponseCallback(ConnectionRequestResponseMessageReceivedHandler callback)
        {
            requestResponseMessageHandler = callback;
        }

        /// <summary>
        /// Removes all callbacks from all channels.
        /// </summary>
        public void ClearChannelCallbacks()
        {
            channelHandlers.Clear();
        }

        /// <summary>
        /// Unregister all callbacks registered to a specific channel. If there are no callbacks registered on the channel, does nothing.
        /// </summary>
        /// <param name="channel">The channel to remove all callbacks from.</param>
        public void UnregisterAllChannelCallbacks(int channel)
        {
            if (channelHandlers.ContainsKey(channel))
            {
                channelHandlers.Remove(channel);
            }
        }

        /// <summary>
        /// Removes the specific callback registered on the given channel. If the callback does not exist, does nothing. 
        /// </summary>
        /// <param name="channel">The channel to remove the callback from.</param>
        /// <param name="callback">The callback to remove.</param>
        public void UnregisterChannelCallback(int channel, ConnectionMessageReceivedHandler callback)
        {
            List<ConnectionMessageReceivedHandler> handlers;

            bool hasHandlers = channelHandlers.TryGetValue(channel, out handlers);

            if (hasHandlers)
            {
                if (handlers.Contains(callback))
                {
                    handlers.Remove(callback);
                }

                if (handlers.Count == 0)
                {
                    channelHandlers.Remove(channel);
                }
            }
        }

        /// <summary>
        /// Unregisters the request response callback associated with the given channel. Does nothing if the channel has no callback.
        /// </summary>
        /// <param name="channel">The channel the callback is registered on.</param>
        public void UnregisterRequestResponseChannelCallback(int channel)
        {
            if (requestResponseChannelHandlers.ContainsKey(channel))
            {
                requestResponseChannelHandlers.Remove(channel);
            }
        }

        /// <summary>
        /// Unregisters the global request response callback.
        /// </summary>
        public void UnregisterRequestResponseCallback()
        {
            requestResponseMessageHandler = null;
        }

        private void OnMessageReceived(ArkeMessage message, ArkeTcpClient client)
        {
            List<ConnectionMessageReceivedHandler> handlers;

            bool hasHandlers = channelHandlers.TryGetValue(message.Channel, out handlers);

            if (hasHandlers)
            {
                handlers.ForEach(callback => callback(message, this));
            }

            MessageReceived?.Invoke(message, this);
        }

        private async Task<ArkeMessage> OnRequestReceived(ArkeMessage message, ArkeTcpClient client)
        {
            ArkeMessage response = null;

            //first we try for a specific channel handler
            ConnectionRequestResponseMessageReceivedHandler handler = null;

            requestResponseChannelHandlers.TryGetValue(message.Channel, out handler);

            if (handler != null)
            {
                response = await handler?.Invoke(message, this);
            }

            //Now we do the generic handler last
            response = await requestResponseMessageHandler?.Invoke(message, this);

            return response;
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

    /// <summary>
    /// Delegate for the Connection Request Response Message Reception
    /// </summary>
    /// <param name="message">The message that was received.</param>
    /// <param name="connection">The connection the message was received on.</param>
    /// <returns>The response message.</returns>
    public delegate Task<ArkeMessage> ConnectionRequestResponseMessageReceivedHandler(ArkeMessage message, ArkeTcpServerConnection connection);

    #endregion
}
