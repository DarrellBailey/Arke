using System;
using System.Text;
using System.Linq;

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

        /// <summary>
        /// Creates an over the wire payload from the current message object.
        /// </summary>
        /// <returns>The message as a byte array</returns>
        internal byte[] GetMessagePayload()
        {
            //get the message channel as an array of bytes
            byte[] channel = BitConverter.GetBytes(Channel);

            //the message length - 4 bytes for length int, 22 bytes for channel, message id guid, control code, and content type header, add payload length
            int length = 4 + 22 + Content.Length;

            //the length in bytes
            byte[] lengthBytes = BitConverter.GetBytes(length);

            //the byte data to transfer
            byte[] transferBytes = new byte[length];

            //add message length
            Array.Copy(lengthBytes, 0, transferBytes, 0, 4);

            //add message channel
            Array.Copy(channel, 0, transferBytes, 4, 4);

            //add control code
            transferBytes[8] = (byte)ControlCode;

            //add content type
            transferBytes[9] = (byte)ContentType;

            //add message id guid
            Array.Copy(MessageId.ToByteArray(), 0, transferBytes, 10, 16);

            //add message content
            Array.Copy(Content, 0, transferBytes, 26, Content.Length);

            return transferBytes;
        }

        /// <summary>
        /// Create an ArkeMessage object from an over the wire payload.
        /// </summary>
        /// <param name="payload">The byte array payload.</param>
        /// <returns>The newly created arke message from the payload.</returns>
        internal static ArkeMessage CreateFromPayload(byte[] payload)
        {
            //the first 4 bytes of the message are the channel
            int channel = BitConverter.ToInt32(payload, 4);

            //the control code is the 5th byte
            ArkeControlCode controlCode = (ArkeControlCode)payload[8];

            //the message type is the 6th byte
            ArkeContentType type = (ArkeContentType)payload[9];

            //the message id is the next 16 bytes
            Guid messageId = new Guid(payload.Skip(10).Take(16).ToArray());

            //the remaining bytes are the payload
            byte[] content = payload.Skip(26).ToArray();

            //create the message object
            return new ArkeMessage(content, channel, type, controlCode, messageId);
        }
    }
}
