using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace Arke
{
    /// <summary>
    /// Arke Tcp Server
    /// </summary>
    public class ArkeTcpServer
    {
        private TcpListener tcpListener;

        private Dictionary<Guid, ArkeTcpServerConnection> internalConnections = new Dictionary<Guid, ArkeTcpServerConnection>();

        private Dictionary<int, List<ServerMessageReceivedHandler>> channelHandlers = new Dictionary<int, List<ServerMessageReceivedHandler>>();

        private Dictionary<int, ServerRequestResponseMessageReceivedHandler> requestResponseChannelHandlers = new Dictionary<int, ServerRequestResponseMessageReceivedHandler>();

        private ServerRequestResponseMessageReceivedHandler requestResponseMessageHandler = null;

        /// <summary>
        /// The servers endpoint.
        /// </summary>
        public IPEndPoint EndPoint { get; private set; }

        /// <summary>
        /// The servers port.
        /// </summary>
        public int Port => EndPoint.Port;

        /// <summary>
        /// The servers address.
        /// </summary>
        public IPAddress Address => EndPoint.Address;

        /// <summary>
        /// Whether or not the server is currently listening.
        /// </summary>
        public bool Listening { get; private set; }

        /// <summary>
        /// All currently active connections.
        /// </summary>
        public ArkeTcpServerConnection[] Connections => internalConnections.Values.ToArray();

        /// <summary>
        /// Create a new Tcp Server listening on all available network addresses at the given port.
        /// </summary>
        /// <param name="port">The port for the server to listen on.</param>
        public ArkeTcpServer(int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Any, port);

            tcpListener = new TcpListener(EndPoint);
        }

        /// <summary>
        /// Create a new Tcp Server listening at the given address and given port. 
        /// </summary>
        /// <param name="address">The address for the server to listen on.</param>
        /// <param name="port">The port for the server to listen on.</param>
        public ArkeTcpServer(IPAddress address, int port)
        {
            EndPoint = new IPEndPoint(address, port);

            tcpListener = new TcpListener(EndPoint);
        }

        /// <summary>
        /// Start the server listening at the given address and port.
        /// </summary>
        public void StartListening()
        {
            if (!Listening)
            {
                tcpListener.Start();

                Task.Run(AcceptSocketLoop);
            }
        }

        /// <summary>
        /// Stop the server listening.
        /// </summary>
        public void StopListening()
        {
            if (Listening)
            {
                tcpListener.Stop();

                Listening = false;
            }
        }

        private async Task AcceptSocketLoop()
        {
            Listening = true;

            while (Listening)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                ArkeTcpClient arkeClient = new ArkeTcpClient(tcpClient);

                ArkeTcpServerConnection connection = new ArkeTcpServerConnection(Guid.NewGuid(), arkeClient, this);

                internalConnections.Add(connection.Id, connection);

                connection.Disconnected += OnDisconnected;

                connection.MessageReceived += OnMessageReceived;

                connection.RegisterRequestResponseCallback(OnRequestReceived);

                connection.StartListening();

                OnConnectionReceived(connection);
            }

            Listening = false;
        }

        /// <summary>
        /// Register a callback for a specific channel. You can register more than one callback on a single channel.
        /// </summary>
        /// <param name="channel">The channel to register to.</param>
        /// <param name="callback">The callback to register.</param>
        public void RegisterChannelCallback(int channel, ServerMessageReceivedHandler callback)
        {
            if (!channelHandlers.ContainsKey(channel))
            {
                channelHandlers.Add(channel, new List<ServerMessageReceivedHandler>());
            }

            List<ServerMessageReceivedHandler> handlers = channelHandlers[channel];

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
        public void RegisterRequestResponseChannelCallback(int channel, ServerRequestResponseMessageReceivedHandler callback)
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
        public void RegisterRequestResponseCallback(ServerRequestResponseMessageReceivedHandler callback)
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
        public void UnregisterChannelCallback(int channel, ServerMessageReceivedHandler callback)
        {
            List<ServerMessageReceivedHandler> handlers;

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

        internal void OnConnectionReceived(ArkeTcpServerConnection connection)
        {
            ConnectionReceived?.Invoke(connection);
        }

        internal void OnMessageReceived(ArkeMessage message, ArkeTcpServerConnection connection)
        {
            List<ServerMessageReceivedHandler> handlers;

            bool hasHandlers = channelHandlers.TryGetValue(message.Channel, out handlers);

            if (hasHandlers)
            {
                handlers.ForEach(callback => callback(message, connection));
            }

            MessageReceived?.Invoke(message, connection);
        }

        private async Task<ArkeMessage> OnRequestReceived(ArkeMessage message, ArkeTcpServerConnection connection)
        {
            ArkeMessage response = null;

            //first we try for a specific channel handler
            ServerRequestResponseMessageReceivedHandler handler = null;

            requestResponseChannelHandlers.TryGetValue(message.Channel, out handler);

            if (handler != null)
            {
                response = await handler?.Invoke(message, connection);
            }

            //Now we do the generic handler last
            response = await requestResponseMessageHandler?.Invoke(message, connection);

            return response;
        }

        private void OnDisconnected(ArkeTcpServerConnection connection)
        {
            if (internalConnections.ContainsKey(connection.Id))
            {
                internalConnections.Remove(connection.Id);
            }

            Disconnected?.Invoke(connection);
        }

        #region Events

        /// <summary>
        /// Event triggered upon initiation of a new connection from client.
        /// </summary>
        public event ServerConnectionReceivedHandler ConnectionReceived;

        /// <summary>
        /// Event triggered when one of the server connections receives a message.
        /// </summary>
        public event ServerMessageReceivedHandler MessageReceived;

        /// <summary>
        /// Triggered when a connection gets disconnected.
        /// </summary>
        public event ConnectionDisconnectedHandler Disconnected;

        #endregion
    }

    #region Delegates

    /// <summary>
    /// The connection received delgate for the ArkeTcpServer connection received event.
    /// </summary>
    /// <param name="connection">The connection that was recieved.</param>
    public delegate void ServerConnectionReceivedHandler(ArkeTcpServerConnection connection);

    /// <summary>
    /// The message received delegate for the ArkeTcpServer message received event.
    /// </summary>
    /// <param name="message">The message that was received.</param>
    /// <param name="connection">The connection that the message was received on.</param>
    public delegate void ServerMessageReceivedHandler(ArkeMessage message, ArkeTcpServerConnection connection);

    /// <summary>
    /// Delegate for the ArkeTcpServer Request Response Message Reception
    /// </summary>
    /// <param name="message">The message that was received.</param>
    /// <param name="connection">The connection the message was received on.</param>
    /// <returns>The response message.</returns>
    public delegate Task<ArkeMessage> ServerRequestResponseMessageReceivedHandler(ArkeMessage message, ArkeTcpServerConnection connection);

    #endregion
}
