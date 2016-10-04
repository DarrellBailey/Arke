namespace Arke.Net
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
        Object = 3
    }
}
