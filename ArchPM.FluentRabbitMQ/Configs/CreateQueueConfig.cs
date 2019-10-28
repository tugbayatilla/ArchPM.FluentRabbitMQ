namespace ArchPM.FluentRabbitMQ.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateQueueConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CreateQueueConfig"/> is exclusive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if exclusive; otherwise, <c>false</c>.
        /// </value>
        public bool Exclusive { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CreateQueueConfig"/> is durable.
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
