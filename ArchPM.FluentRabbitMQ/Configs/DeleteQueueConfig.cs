namespace ArchPM.FluentRabbitMQ.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteQueueConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether [if unused].
        /// Default is false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [if unused]; otherwise, <c>false</c>.
        /// </value>
        public bool IfUnused { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [if unused].
        /// Default is false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [if unused]; otherwise, <c>false</c>.
        /// </value>
        public bool IfEmpty { get; set; }
    }
}
