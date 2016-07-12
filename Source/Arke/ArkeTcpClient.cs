using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Arke
{
    public class ArkeTcpClient
    {
        const int defaultBufferSize = 8192;

        protected byte[] readBuffer = new byte[defaultBufferSize];

        protected List<byte> messageBuffer = new List<byte>(defaultBufferSize);

        protected Task listenTask = null;

        protected CancellationTokenSource listenCts = new CancellationTokenSource();

        protected Dictionary<int, List<ClientMessageReceivedHandler>> channelHandlers = new Dictionary<int, List<ClientMessageReceivedHandler>>();

        protected Action<byte[]> processMessage;

        /// <summary>
        /// The underlying Tcp Client object for this Arke Client.
        /// </summary>
        public TcpClient TcpClient { get; protected set; }

        /// <summary>
        /// Whether or not the client is connected.
        /// </summary>
        public bool Connected => TcpClient.Connected;

        /// <summary>
        /// Creaqte a new ArkeTcpClient.
        /// </summary>
        public ArkeTcpClient()
        {
            TcpClient = new TcpClient();
        }

        /// <summary>
        /// Create an ArkeTcpClient from an existing TcpClient object.
        /// </summary>
        /// <param name="client">The existing TcpClient object.</param>
        public ArkeTcpClient(TcpClient client)
        {
            TcpClient = client;
        }

        /// <summary>
        /// Connect to the given host at the given port.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect on.</param>
        public void Connect(string host, int port)
        {
            TcpClient.ConnectAsync(host, port).Wait();

            StartListening();
        }

        /// <summary>
        /// Connect to the given host at the given port.
        /// </summary>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect on.</param>
        public async Task ConnectAsync(string host, int port)
        {
            await TcpClient.ConnectAsync(host, port);

            StartListening();
        }

        /// <summary>
        /// Connect to the given address at the given port.
        /// </summary>
        /// <param name="address">The address to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        public void Connect(IPAddress address, int port)
        {
            TcpClient.ConnectAsync(address, port).Wait();

            StartListening();
        }

        /// <summary>
        /// Connect to the given address at the given port.
        /// </summary>
        /// <param name="address">The address to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        public async Task ConnectAsync(IPAddress address, int port)
        {
            await TcpClient.ConnectAsync(address, port);

            StartListening();
        }

        internal void StartListening()
        {
            listenTask = Task.Run(Listen, listenCts.Token);
        }

        protected async Task Listen()
        {
            //get network stream
            NetworkStream stream = TcpClient.GetStream();

            //enter listen loop
            while (Connected && !listenCts.IsCancellationRequested)
            {
                int bytesRead = await stream.ReadAsync(readBuffer, 0, readBuffer.Length);

                messageBuffer.AddRange(readBuffer.Take(bytesRead));

                ProcessMessageBuffer();
            }
        }

        protected void ProcessMessageBuffer()
        {
            //it is possible to have more than one message in the buffer, so process all available messages
            while (messageBuffer.Count >= 4)
            {
                //the first 4 bytes of the message are the message length. This value includes the 4 bytes for the message length itself.
                int messageLength = BitConverter.ToInt32(messageBuffer.Take(4).ToArray(), 0);

                //if the message buffer does not contain the entire message, move on.
                if (messageBuffer.Count() < messageLength) break;

                //get the message data from the buffer
                byte[] messageBytes = messageBuffer.Skip(4).Take(messageLength - 4).ToArray();

                //clear the message out of the buffer
                messageBuffer.RemoveRange(0, messageLength);

                //Process the message
                Task processMessage = new Task(new Action<object>(ProcessMessage), messageBytes);

                processMessage.Start();
            }
        }

        protected void ProcessMessage(object obj)
        {
            //cast obj to byte array like it should be
            byte[] message = (byte[])obj;

            //the first 4 bytes of the message are the channel
            int channel = BitConverter.ToInt32(message, 0);

            //the message type is the 5th byte
            ArkeContentType type = (ArkeContentType)message[4];

            //the remaining bytes are the payload
            byte[] payload = message.Skip(5).Take(message.Length - 5).ToArray();

            //create the message object
            ArkeMessage messageObject = new ArkeMessage(payload, channel, type);

            //trigger event
            OnMessageReceived(messageObject);
        }

        protected void OnMessageReceived(ArkeMessage message)
        {
            List<ClientMessageReceivedHandler> handlers;

            bool hasHandlers = channelHandlers.TryGetValue(message.Channel, out handlers);

            if (hasHandlers)
            {
                handlers.ForEach(callback => callback(message));
            }

            MessageReceived?.Invoke(message);
        }

        /// <summary>
        /// Send the given message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(ArkeMessage message)
        {
            if (!Connected) throw new ArkeException("Attempt to send data on a disconnected client is not allowed.");

            byte[] transferBytes = PrepareMessageForSend(message);

            TcpClient.GetStream().Write(transferBytes, 0, transferBytes.Length);
        }

        /// <summary>
        /// Send the given message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public async Task SendAsync(ArkeMessage message)
        {
            if (!Connected) throw new ArkeException("Attempt to send data on a disconnected client is not allowed.");

            byte[] transferBytes = PrepareMessageForSend(message);

            await TcpClient.GetStream().WriteAsync(transferBytes, 0, transferBytes.Length);
        }

        protected byte[] PrepareMessageForSend(ArkeMessage message)
        {
            //get the message channel as an array of bytes
            byte[] channel = BitConverter.GetBytes(message.Channel);

            //get the message type as a byte
            byte type = (byte)message.ContentType;

            //the message length - 4 bytes for length int, 5 bytes for channel and content type header, add payload length
            int length = 4 + 5 + message.Content.Count();

            //the length in bytes
            byte[] lengthBytes = BitConverter.GetBytes(length);

            //the byte data to transfer
            byte[] transferBytes = new byte[length];

            Array.Copy(lengthBytes, 0, transferBytes, 0, 4);

            Array.Copy(channel, 0, transferBytes, 4, 4);

            transferBytes[8] = type;

            Array.Copy(message.Content, 0, transferBytes, 9, message.Content.Length);

            return transferBytes;
        }

        /// <summary>
        /// Register a callback for a specific channel. You can register more than one callback on a single channel.
        /// </summary>
        /// <param name="channel">The channel to register to.</param>
        /// <param name="callback">The callback to register.</param>
        public void RegisterChannelCallback(int channel, ClientMessageReceivedHandler callback)
        {
            if (!channelHandlers.ContainsKey(channel))
            {
                channelHandlers.Add(channel, new List<ClientMessageReceivedHandler>());
            }

            List<ClientMessageReceivedHandler> handlers = channelHandlers[channel];

            if (!handlers.Contains(callback))
            {
                handlers.Add(callback);
            }
        }

        #region Events

        public event ClientMessageReceivedHandler MessageReceived;

        #endregion
    }

    public delegate void ClientMessageReceivedHandler(ArkeMessage message);
}
