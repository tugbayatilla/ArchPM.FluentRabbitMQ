using ArchPM.FluentRabbitMQ.Configs.Infos;

namespace ArchPM.FluentRabbitMQ.Configs
{
    /// <summary>
    /// 
    /// </summary>
    public class FluentRabbitConfiguration
    {
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public ConnectionInfo Connection { get; set; } = new ConnectionInfo();

        /// <summary>
        /// Gets the exchanges.
        /// </summary>
        /// <value>
        /// The exchanges.
        /// </value>
        public ExchangeInfos Exchanges { get; } = new ExchangeInfos();

        /// <summary>
        /// Gets the queues.
        /// </summary>
        /// <value>
        /// The queues.
        /// </value>
        public QueueInfos Queues { get; } = new QueueInfos();

        /// <summary>
        /// Gets the bindings.
        /// </summary>
        /// <value>
        /// The bindings.
        /// </value>
        public BindingInfos Bindings { get; } = new BindingInfos();
    }
}