using System;
using System.Collections.Generic;
using ArchPM.FluentRabbitMQ.Configs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ArchPM.FluentRabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFluentRabbit
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        FluentRabbitConfiguration Configuration { get; }
        /// <summary>
        /// Gets the rabbit mq client.
        /// </summary>
        /// <value>
        /// The rabbit mq client.
        /// </value>
        RabbitMqClient RabbitMqClient { get; }

        /// <summary>
        /// Binds the specified configuration action.
        /// </summary>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit Bind(Action<BindingConfig> configAction);
        /// <summary>
        /// Binds the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit Bind(BindingConfig config);
        /// <summary>
        /// Binds the specified exchange name.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        IFluentRabbit Bind(string exchangeName, string queueName, string routingKey, IDictionary<string, object> arguments = null);
        /// <summary>
        /// Configures the specified configuration action.
        /// </summary>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit Configure(Action<FluentRabbitConfiguration> configAction);
        /// <summary>
        /// Configures the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit Configure(FluentRabbitConfiguration config);
        /// <summary>
        /// Configures down.
        /// </summary>
        /// <returns></returns>
        IFluentRabbit ConfigureDown();
        /// <summary>
        /// Configures up.
        /// </summary>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit ConfigureUp(Action<FluentRabbitConfiguration> configAction = null);
        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        IFluentRabbit Connect();
        /// <summary>
        /// Creates the exchange.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit CreateExchange(string exchangeName, Action<CreateExchangeConfig> configAction = null);
        /// <summary>
        /// Creates the exchange.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit CreateExchange(string exchangeName, CreateExchangeConfig config);
        /// <summary>
        /// Creates the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit CreateQueue(string queueName, Action<CreateQueueConfig> configAction = null);
        /// <summary>
        /// Creates the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit CreateQueue(string queueName, CreateQueueConfig config);
        /// <summary>
        /// Deletes the exchange.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit DeleteExchange(string exchangeName, Action<DeleteExchangeConfig> configAction = null);
        /// <summary>
        /// Deletes the exchange.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit DeleteExchange(string exchangeName, DeleteExchangeConfig config);
        /// <summary>
        /// Deletes the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit DeleteQueue(string queueName, Action<DeleteQueueConfig> configAction = null);
        /// <summary>
        /// Deletes the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit DeleteQueue(string queueName, DeleteQueueConfig config);
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        void Dispose();
        /// <summary>
        /// Fetches the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit Fetch(string queueName, Action<BasicGetResult> callback, Action<FetchConfig> configAction = null);
        /// <summary>
        /// Fetches the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit Fetch(string queueName, Action<BasicGetResult> callback, FetchConfig config);
        /// <summary>
        /// Publishes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit Publish(byte[] data, PublishConfig config);
        /// <summary>
        /// Publishes the specified payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload">The payload.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit Publish<T>(T payload, Action<PublishConfig> configAction);
        /// <summary>
        /// Publishes the specified payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload">The payload.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit Publish<T>(T payload, PublishConfig config);
        /// <summary>
        /// Purges the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="resultAction">The result action.</param>
        /// <returns></returns>
        IFluentRabbit PurgeQueue(string queueName, Action<uint> resultAction = null);
        /// <summary>
        /// Sleeps the specified frequency.
        /// </summary>
        /// <param name="frequency">The frequency.</param>
        /// <returns></returns>
        IFluentRabbit Sleep(int frequency = 1000);
        /// <summary>
        /// Subscribes the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit Subscribe(string queueName, Action<BasicDeliverEventArgs> callback, Action<SubscribeConfig> configAction = null);
        /// <summary>
        /// Subscribes the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit Subscribe(string queueName, Action<BasicDeliverEventArgs> callback, SubscribeConfig config);
        /// <summary>
        /// Traces the specified trace action.
        /// </summary>
        /// <param name="traceAction">The trace action.</param>
        /// <returns></returns>
        IFluentRabbit Trace(Action<TraceData> traceAction);
        /// <summary>
        /// Unbinds the specified configuration action.
        /// </summary>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit Unbind(Action<BindingConfig> configAction);
        /// <summary>
        /// Unbinds the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IFluentRabbit Unbind(BindingConfig config);
        /// <summary>
        /// Unbinds the specified exchange name.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        IFluentRabbit Unbind(string exchangeName, string queueName, string routingKey, IDictionary<string, object> arguments = null);
        /// <summary>
        /// Waits the until.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="frequency">The frequency.</param>
        /// <returns></returns>
        IFluentRabbit WaitUntil(Func<bool> condition, int timeout = 1000, int frequency = 25);
        /// <summary>
        /// Waits the until.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        IFluentRabbit WaitUntil(Func<bool> condition, Action<WaitUntilConfig> configAction);
    }
}