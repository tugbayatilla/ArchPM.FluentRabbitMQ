using System.Collections.Generic;

namespace ArchPM.FluentRabbitMQ.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public class SubscribeConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether [automatic ack].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic ack]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoAck { get; set; } = false;

        /// <summary>
        /// Gets or sets the consumer tag.
        /// </summary>
        /// <value>
        /// The consumer tag.
        /// </value>
        public string ConsumerTag { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether [no local].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [no local]; otherwise, <c>false</c>.
        /// </value>
        public bool NoLocal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SubscribeConfig"/> is exclusive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if exclusive; otherwise, <c>false</c>.
        /// </value>
        public bool Exclusive { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public IDictionary<string, object> Arguments { get; set; }
    }
}