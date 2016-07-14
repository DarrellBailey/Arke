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
    /// <summary>
    /// Arke Tcp Client
    /// </summary>
    public class ArkeTcpClient : IDisposable
    {
        const int defaultBufferSize = 8192;

        private byte[] readBuffer = new byte[defaultBufferSize];

        private List<byte> messageBuffer = new List<byte>(defaultBufferSize);

        private Task listenTask = null;

        private CancellationTokenSource listenCts = new CancellationTokenSource();

        private Dictionary<int, List<ClientMessageReceivedHandler>> channelHandlers = new Dictionary<int, List<ClientMessageReceivedHandler>>();

        /// <summary>
        /// The underlying Tcp Client object for this Arke Client.
        /// </summary>
        public TcpClient TcpClient { get; private set; }

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

        private async Task Listen()
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

        private void ProcessMessageBuffer()
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

        private void ProcessMessage(object obj)
        {
            //cast obj to byte array like it should be
            byte[] message = (byte[])obj;

            //the first 4 bytes of the message are the channel
            int channel = BitConverter.ToInt32(message, 0);

            //the control code is the 5th byte
            ArkeControlCode controlCode = (ArkeControlCode)message[4];

            //the message type is the 6th byte
            ArkeContentType type = (ArkeContentType)message[5];

            //the message id is the next 16 bytes
            Guid messageId = new Guid(message.Skip(6).Take(16).ToArray());

            //the remaining bytes are the payload
            byte[] payload = message.Skip(22).ToArray();

            //create the message object
            ArkeMessage messageObject = new ArkeMessage(payload, channel, type);

            //trigger event
            OnMessageReceived(messageObject);
        }

        private void OnMessageReceived(ArkeMessage message)
        {
            List<ClientMessageReceivedHandler> handlers;

            bool hasHandlers = channelHandlers.TryGetValue(message.Channel, out handlers);

            if (hasHandlers)
            {
                handlers.ForEach(callback => callback(message, this));
            }

            MessageReceived?.Invoke(message, this);
        }

        /// <summary>
        /// Send the given message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void Send(ArkeMessage message)
        {
            if (!Connected) throw new ArkeException("Attempt to send data on a disconnected client is not allowed.");

            SendAsync(message).Wait();
        }

        /// <summary>
        /// Send the given message.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public async Task SendAsync(ArkeMessage message)
        {
            if (!Connected) throw new ArkeException("Attempt to send data on a disconnected client is not allowed.");

            byte[] transferBytes = PrepareMessageForSend(message, ArkeControlCode.Message);

            await TcpClient.GetStream().WriteAsync(transferBytes, 0, transferBytes.Length);
        }

        private byte[] PrepareMessageForSend(ArkeMessage message, ArkeControlCode controlCode)
        {
            Guid messageId = Guid.NewGuid();

            //get the message channel as an array of bytes
            byte[] channel = BitConverter.GetBytes(message.Channel);

            //the message length - 4 bytes for length int, 22 bytes for control code, channel, guid, and content type header, add payload length
            int length = 4 + 22 + message.Content.Count();

            //the length in bytes
            byte[] lengthBytes = BitConverter.GetBytes(length);

            //the byte data to transfer
            byte[] transferBytes = new byte[length];

            Array.Copy(lengthBytes, 0, transferBytes, 0, 4);

            Array.Copy(channel, 0, transferBytes, 4, 4);

            transferBytes[8] = (byte)controlCode;

            transferBytes[9] = (byte)message.ContentType;

            Array.Copy(messageId.ToByteArray(), 0, transferBytes, 10, 16);

            Array.Copy(message.Content, 0, transferBytes, 26, message.Content.Length);

            return transferBytes;
        }

        /// <summary>
        /// Register a callback for a specific channel. You can register more than one callback on a single channel.
        /// </summary>
        /// <param name="channel">The channel to register to.</param>
        /// <param name="callback">The callback to register.</param>
        public void RegisterChannelCallback(int channel, ClientMessageReceivedHandler callback)        {
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

        /// <summary>
        /// Triggered when this client receives a message.
        /// </summary>
        public event ClientMessageReceivedHandler MessageReceived;

        #endregion

        #region IDisposable

        private bool disposed = false; // To detect redundant calls

        /// <summary>
        /// Clean up the listen thread and tcp client.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    TcpClient.Dispose();
                    listenCts.Cancel();
                    listenCts.Dispose();
                }               

                disposed = true;
            }
        }

        /// <summary>
        /// Disposes the ArkeTcpClient
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }

    /// <summary>
    /// Event handler delegate for the ArkeTcpClient MessageReceived event.
    /// </summary>
    /// <param name="message">The message that was received.</param>
    /// <param name="client">The client the message was received on.</param>
    public delegate void ClientMessageReceivedHandler(ArkeMessage message, ArkeTcpClient client);

    /// <summary>
    /// Event handler delegate for the ArkeTcpClient Disconnected event.
    /// </summary>
    /// <param name="client">The client that was disconnected.</param>
    public delegate void ClientDisconnectedHandler(ArkeTcpClient client);
}
