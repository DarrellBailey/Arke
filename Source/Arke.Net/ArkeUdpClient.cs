using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.Net
{
    /// <summary>
    /// Arke Client for the Udp protocol
    /// </summary>
    public class ArkeUdpClient
    {
        /// <summary>
        /// The underlying udp client object for the Arke Client.
        /// </summary>
        internal UdpClient UdpClient { get; private set; }

        /// <summary>
        /// Create a new Arke Udp Client listening at the given port.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        public ArkeUdpClient(int port)
        {
            UdpClient = new UdpClient(port);
        }

        #region Events

        /// <summary>
        /// Triggered when this client receives a message.
        /// </summary>
        public event UdpClientMessageReceivedHandler MessageReceived;

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
    /// Event handler delegate for the ArkeUdpClient MessageReceived event.
    /// </summary>
    /// <param name="message">The message that was received.</param>
    /// <param name="client">The client the message was received on.</param>
    public delegate void UdpClientMessageReceivedHandler(ArkeMessage message, ArkeUdpClient client);

    /// <summary>
    /// Delegate for the Client Request Response Message Reception
    /// </summary>
    /// <param name="message">The message that was received.</param>
    /// <param name="client">The client the message was received on.</param>
    /// <returns>The response message.</returns>
    public delegate Task<ArkeMessage> UdpClientRequestResponseMessageReceivedHandler(ArkeMessage message, ArkeUdpClient client);
}
