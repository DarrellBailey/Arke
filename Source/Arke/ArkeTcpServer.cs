﻿using System;
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
        private TcpListener TcpListener;

        private Dictionary<Guid, ArkeTcpServerConnection> InternalConnections = new Dictionary<Guid, ArkeTcpServerConnection>();

        private Dictionary<int, List<ServerMessageReceivedHandler>> ChannelHandlers = new Dictionary<int, List<ServerMessageReceivedHandler>>();

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
        public ArkeTcpServerConnection[] Connections => InternalConnections.Values.ToArray();

        /// <summary>
        /// Create a new Tcp Server listening on all available network addresses at the given port.
        /// </summary>
        /// <param name="port">The port for the server to listen on.</param>
        public ArkeTcpServer(int port)
        {
            EndPoint = new IPEndPoint(IPAddress.Any, port);

            TcpListener = new TcpListener(EndPoint);
        }

        /// <summary>
        /// Create a new Tcp Server listening at the given address and given port. 
        /// </summary>
        /// <param name="address">The address for the server to listen on.</param>
        /// <param name="port">The port for the server to listen on.</param>
        public ArkeTcpServer(IPAddress address, int port)
        {
            EndPoint = new IPEndPoint(address, port);

            TcpListener = new TcpListener(EndPoint);
        }

        /// <summary>
        /// Start the server listening at the given address and port.
        /// </summary>
        public void StartListening()
        {
            if (!Listening)
            {
                TcpListener.Start();

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
                TcpListener.Stop();

                Listening = false;
            }
        }

        private async Task AcceptSocketLoop()
        {
            Listening = true;

            while (Listening)
            {
                TcpClient tcpClient = await TcpListener.AcceptTcpClientAsync();

                ArkeTcpClient arkeClient = new ArkeTcpClient(tcpClient);

                ArkeTcpServerConnection connection = new ArkeTcpServerConnection(Guid.NewGuid(), arkeClient, this);

                InternalConnections.Add(connection.Id, connection);

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
            if (!ChannelHandlers.ContainsKey(channel))
            {
                ChannelHandlers.Add(channel, new List<ServerMessageReceivedHandler>());
            }

            List<ServerMessageReceivedHandler> handlers = ChannelHandlers[channel];

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

        internal void OnConnectionReceived(ArkeTcpServerConnection connection)
        {
            ConnectionReceived?.Invoke(connection);
        }

        internal void OnMessageReceived(ArkeMessage message, ArkeTcpServerConnection connection)
        {
            List<ServerMessageReceivedHandler> handlers;

            bool hasHandlers = ChannelHandlers.TryGetValue(message.Channel, out handlers);

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

        #region Events

        /// <summary>
        /// Event triggered upon initiation of a new connection from client.
        /// </summary>
        public event ServerConnectionReceivedHandler ConnectionReceived;

        /// <summary>
        /// Event triggered when one of the server connections receives a message.
        /// </summary>
        public event ServerMessageReceivedHandler MessageReceived;

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
