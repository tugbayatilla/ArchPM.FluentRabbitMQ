namespace ArchPM.FluentRabbitMQ.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateExchangeConfig
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; } = ExchangeType.Direct;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CreateExchangeConfig"/> is durable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if durable; otherwise, <c>false</c>.
        /// </value>
        public bool Durable { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether [automatic delete].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic delete]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoDelete { get; set; } = false;

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public ArgumentsDictionary Arguments { get; } = new ArgumentsDictionary();


    }
}
