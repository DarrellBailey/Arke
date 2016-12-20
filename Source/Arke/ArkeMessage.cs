using System;
using System.Text;

namespace Arke
{
    /// <summary>
    /// Arke Message Object. Used to construct messages between Arke endpoints.
    /// </summary>
    public class ArkeMessage
    {
        internal byte[] Content { get; set; }

        internal ArkeControlCode ControlCode { get; set; }

        internal Guid MessageId { get; set; }

        /// <summary>
        /// The channel that the message will be sent on.
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// The content type of the message.
        /// </summary>
        public ArkeContentType ContentType { get; set; }

        /// <summary>
        /// Create a new message with the default channel 0 and empty content.
        /// </summary>
        public ArkeMessage()
        {
            ContentType = ArkeContentType.None;

            Channel = 0;

            Content = new byte[0];

            MessageId = Guid.NewGuid();
        }

        /// <summary>
        /// Create a new message with the given content and optional channel.
        /// </summary>
        /// <param name="content">The message content.</param>
        /// <param name="channel">The message channel</param>
        public ArkeMessage(byte[] content, int channel = 0)
        {
            SetContent(content);

            Channel = channel;

            MessageId = Guid.NewGuid();
        }

        /// <summary>
        /// Create a new message with the given content and optional channel.
        /// </summary>
        /// <param name="content">The message content.</param>
        /// <param name="channel">The message channel</param>
        public ArkeMessage(string content, int channel = 0)
        {
            SetContent(content);

            Channel = channel;

            MessageId = Guid.NewGuid();
        }

        internal ArkeMessage(byte[] content, int channel, ArkeContentType type)
        {
            ContentType = type;

            Channel = channel;

            Content = content;

            MessageId = Guid.NewGuid();
        }

        internal ArkeMessage(byte[] content, int channel, ArkeContentType type, ArkeControlCode controlCode, Guid messageId) : this(content, channel, type)
        {
            ContentType = type;

            Channel = channel;

            Content = content;

            ControlCode = controlCode;

            MessageId = messageId;
        }

        /// <summary>
        /// Sets the message content from an array of bytes.
        /// </summary>
        /// <param name="content">The byte array to set as the message content.</param>
        public void SetContent(byte[] content)
        {
            Content = content;

            ContentType = ArkeContentType.Bytes;
        }

        /// <summary>
        /// Sets the message content from a string.
        /// </summary>
        /// <param name="content">The string to set as the message content.</param>
        public void SetContent(string content)
        {
            Content = Encoding.UTF8.GetBytes(content);

            ContentType = ArkeContentType.String;
        }

        /// <summary>
        /// Get the message content as an array of bytes.
        /// </summary>
        /// <returns>The message content as an array of bytes.</returns>
        public byte[] GetContentAsBytes()
        {
            return Content;
        }

        /// <summary>
        /// Get the message content as a string.
        /// </summary>
        /// <returns>The message content as a string.</returns>
        public string GetContentAsString()
        {
            return Encoding.UTF8.GetString(Content);
        }
    }
}
