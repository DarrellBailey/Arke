namespace Arke
{
    /// <summary>
    /// The content type of an ArkeMessage.
    /// </summary>
    public enum ArkeContentType
    {
        /// <summary>
        /// Unset or Unknown content type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Byte Array content type.
        /// </summary>
        Bytes = 1,
        /// <summary>
        /// String content type.
        /// </summary>
        String = 2,
        /// <summary>
        /// Object content type.
        /// </summary>
        Object = 3,
        /// <summary>
        /// Json Content Type
        /// </summary>
        Json = 4,
        /// <summary>
        /// Xml Content Type
        /// </summary>
        Xml = 5,
        /// <summary>
        /// Form Data Content Type
        /// </summary>
        FormData = 6,
        /// <summary>
        /// Stream Content Type
        /// </summary>
        Stream = 7,
        /// <summary>
        /// Custom Content Type
        /// </summary>
        Custom = 100,
    }
}
