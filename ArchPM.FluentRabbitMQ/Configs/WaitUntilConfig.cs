namespace ArchPM.FluentRabbitMQ.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public class WaitUntilConfig
    {
        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>
        /// The timeout.
        /// </value>
        public int Timeout { get; set; } = 1000;
        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>
        /// The frequency.
        /// </value>
        public int Frequency { get; set; } = 25;
        /// <summary>
        /// Gets or sets a value indicating whether [throw time exception when timeout reached].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [throw time exception when timeout reached]; otherwise, <c>false</c>.
        /// </value>
        public bool ThrowTimeExceptionWhenTimeoutReached { get; set; } = true;
    }
}
