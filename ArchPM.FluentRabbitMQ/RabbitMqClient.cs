using RabbitMQ.Client;

namespace ArchPM.FluentRabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitMqClient
    {
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public IModel Model { get; internal set; }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public IConnection Connection { get; internal set; }

        /// <summary>
        /// Gets the connection factory.
        /// </summary>
        /// <value>
        /// The connection factory.
        /// </value>
        public IConnectionFactory ConnectionFactory { get; internal set; }

    }
}