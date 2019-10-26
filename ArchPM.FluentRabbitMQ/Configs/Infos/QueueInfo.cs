namespace ArchPM.FluentRabbitMQ.Configs.Infos
{
    /// <summary>
    /// 
    /// </summary>
    public class QueueInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public CreateQueueConfig Config { get; set; } = new CreateQueueConfig();
    }
}
