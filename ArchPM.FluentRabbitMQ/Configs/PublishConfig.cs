using ArchPM.NetCore.Extensions;
using RabbitMQ.Client;

namespace ArchPM.FluentRabbitMQ.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public class PublishConfig
    {
        /// <summary>
        /// Gets or sets the name of the exchange.
        /// </summary>
        /// <value>
        /// The name of the exchange.
        /// </value>
        public string ExchangeName { get; set; }
        /// <summary>
        /// Gets or sets the routing key.
        /// </summary>
        /// <value>
        /// The routing key.
        /// </value>
        public string RoutingKey { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PublishConfig"/> is mandatory.
        /// Default is false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if mandatory; otherwise, <c>false</c>.
        /// </value>
        public bool Mandatory { get; set; } = false;
        /// <summary>
        /// Gets or sets the basic properties.
        /// </summary>
        /// <value>
        /// The basic properties.
        /// </value>
        public IBasicProperties BasicProperties { get; set; }
        /// <summary>
        /// Gets or sets the publish method.
        /// Default is PayloadFormat.Json.
        /// </summary>
        /// <value>
        /// The publish method.
        /// </value>
        public PayloadFormat PayloadFormat { get; set; } = PayloadFormat.Json;

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public void Validate()
        {
            ExchangeName.ThrowExceptionIf(string.IsNullOrWhiteSpace, $"{nameof(ExchangeName)} is null.");
            RoutingKey.ThrowExceptionIf(string.IsNullOrWhiteSpace, $"{nameof(RoutingKey)} is null.");
        }
    }
}