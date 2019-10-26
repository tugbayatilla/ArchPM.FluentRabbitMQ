namespace ArchPM.FluentRabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    public enum PayloadFormat
    {
        /// <summary>
        /// The string
        /// </summary>
        String,
        /// <summary>
        /// The json
        /// </summary>
        Json,
        /// <summary>
        /// The XML
        /// </summary>
        Xml,
        /// <summary>
        /// The byte array
        /// </summary>
        ByteArray
    }

    /// <summary>
    /// 
    /// </summary>
    public struct ExchangeType
    {
        /// <summary>
        /// The direct
        /// </summary>
        public const string Direct = "direct";
        /// <summary>
        /// The fanout
        /// </summary>
        public const string Fanout = "fanout";
        /// <summary>
        /// The topic
        /// </summary>
        public const string Topic = "topic";
        /// <summary>
        /// The headers
        /// </summary>
        public const string Headers = "headers";
    }
}
