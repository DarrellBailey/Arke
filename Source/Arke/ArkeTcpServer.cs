using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace Arke
{
    public class ArkeTcpServer
    {
        protected TcpListener TcpListener;

        protected Dictionary<Guid, ArkeTcpServerConnection> InternalConnections = new Dictionary<Guid, ArkeTcpServerConnection>();

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

                Listening = true;
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

        protected async Task AcceptSocketLoop()
        {
            while (Listening)
            {
                TcpClient tcpClient = await TcpListener.AcceptTcpClientAsync();

                ArkeTcpClient arkeClient = new ArkeTcpClient(tcpClient);

                ArkeTcpServerConnection connection = new ArkeTcpServerConnection(Guid.NewGuid(), arkeClient, this);

                InternalConnections.Add(connection.Id, connection);

                connection.MessageReceived += OnMessageReceived;

                OnConnectionReceived(connection);

                connection.StartListening();                
            }
        }

        internal void OnConnectionReceived(ArkeTcpServerConnection connection)
        {
            ConnectionReceived?.Invoke(connection);
        }

        internal void OnMessageReceived(ArkeMessage message, ArkeTcpServerConnection connection)
        {
            MessageReceived?.Invoke(message, connection);
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

    public delegate void ServerConnectionReceivedHandler(ArkeTcpServerConnection connection);

    public delegate void ServerMessageReceivedHandler(ArkeMessage message, ArkeTcpServerConnection connection);

    #endregion
}
