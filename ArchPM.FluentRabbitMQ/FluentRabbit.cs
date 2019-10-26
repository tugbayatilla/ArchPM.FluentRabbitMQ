using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using ArchPM.FluentRabbitMQ.Configs;
using ArchPM.FluentRabbitMQ.Configs.Infos;
using ArchPM.FluentRabbitMQ.Exceptions;
using ArchPM.NetCore.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ArchPM.FluentRabbitMQ
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class FluentRabbit : IDisposable
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public FluentRabbitConfiguration Configuration { get; } = new FluentRabbitConfiguration();

        /// <summary>
        /// Gets the rabbit mq client.
        /// </summary>
        /// <value>
        /// The rabbit mq client.
        /// </value>
        public RabbitMqClient RabbitMqClient { get; } = new RabbitMqClient();

        /// <summary>
        /// Configures the specified configuration action.
        /// </summary>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit Configure(Action<FluentRabbitConfiguration> configAction)
        {
            configAction?.Invoke(Configuration);

            return this;
        }

        /// <summary>
        /// Configures up.
        /// </summary>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit ConfigureUp(Action<FluentRabbitConfiguration> configAction = null)
        {
            configAction?.Invoke(Configuration);

            foreach (var exchange in Configuration.Exchanges)
            {
                CreateExchange(exchange.Name, exchange.Config);
            }
            foreach (var queue in Configuration.Queues)
            {
                CreateQueue(queue.Name, queue.Config);
            }
            foreach (var binding in Configuration.Bindings)
            {
                Bind(binding.Config);
            }


            return this;
        }

        /// <summary>
        /// Configures down.
        /// </summary>
        /// <returns></returns>
        public FluentRabbit ConfigureDown()
        {
            foreach (var exchange in Configuration.Exchanges)
            {
                DeleteExchange(
                    exchange.Name,
                    new DeleteExchangeConfig() { IfUnused = false }
                );
            }
            foreach (var queue in Configuration.Queues)
            {
                DeleteQueue(
                    queue.Name,
                    new DeleteQueueConfig()
                    { IfUnused = false, IfEmpty = false }
                );
            }
            foreach (var binding in Configuration.Bindings)
            {
                Unbind(binding.Config);
            }

            return this;
        }


        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        public FluentRabbit Connect()
        {
            RabbitMqClient.ConnectionFactory = new ConnectionFactory()
            {
                HostName = Configuration.Connection.Host,
                Password = Configuration.Connection.Password,
                Port = Configuration.Connection.Port,
                UserName = Configuration.Connection.Username,
                VirtualHost = Configuration.Connection.VirtualHost
            };

            RabbitMqClient.Connection = RabbitMqClient.ConnectionFactory.CreateConnection();
            RabbitMqClient.Model = RabbitMqClient.Connection.CreateModel();

            return this;
        }


        #region Create Exchange
        /// <summary>
        /// Creates the exchange.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public FluentRabbit CreateExchange(string exchangeName, CreateExchangeConfig config)
        {
            //validation
            config.ThrowExceptionIfNull<ArgumentNullException>(nameof(config));
            RabbitMqClient.Model.ThrowExceptionIfNull<ModelIsNullException>();

            //execution
            RabbitMqClient.Model.ExchangeDeclare(exchangeName, config.Type, config.Durable, config.AutoDelete, config.Arguments);

            //result
            return this;
        }

        /// <summary>
        /// Creates the exchange.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit CreateExchange(string exchangeName, Action<CreateExchangeConfig> configAction = null)
        {
            var exchangeInfo = Configuration.Exchanges.FirstOrDefault(p => p.Name == exchangeName) ??
                new ExchangeInfo();

            configAction?.Invoke(exchangeInfo.Config);

            return CreateExchange(exchangeName, exchangeInfo.Config); //todo: like this
        }
        #endregion

        #region Create Queue
        /// <summary>
        /// Creates the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public FluentRabbit CreateQueue(string queueName, CreateQueueConfig config)
        {
            //validation
            config.ThrowExceptionIfNull<ArgumentNullException>(nameof(config));
            RabbitMqClient.Model.ThrowExceptionIfNull<ModelIsNullException>();

            //execution
            RabbitMqClient.Model.QueueDeclare(queueName, config.Durable, config.Exclusive, config.AutoDelete, config.Arguments);

            //result
            return this;
        }

        /// <summary>
        /// Creates the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit CreateQueue(string queueName, Action<CreateQueueConfig> configAction = null)
        {
            var queueInfo = Configuration.Queues.FirstOrDefault(p => p.Name == queueName) ?? new QueueInfo();
            configAction?.Invoke(queueInfo.Config);

            return CreateQueue(queueName, queueInfo.Config);
        }
        #endregion

        #region Bind / Unbind
        /// <summary>
        /// Binds the specified exchange name.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public FluentRabbit Bind(string exchangeName, string queueName, string routingKey, IDictionary<string, object> arguments = null)
        {
            RabbitMqClient.Model.ThrowExceptionIfNull<ModelIsNullException>();
            RabbitMqClient.Model.QueueBind(queueName, exchangeName, routingKey, arguments);
            return this;
        }

        /// <summary>
        /// Binds the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public FluentRabbit Bind(BindingConfig config)
        {
            //validation
            config.ThrowExceptionIfNull<ArgumentNullException>(nameof(config));
            config.ExchangeName.ThrowExceptionIf(string.IsNullOrWhiteSpace, $"{nameof(config.ExchangeName)} is null.");
            config.QueueName.ThrowExceptionIf(string.IsNullOrWhiteSpace, $"{nameof(config.QueueName)} is null.");
            config.RoutingKey.ThrowExceptionIf(string.IsNullOrWhiteSpace, $"{nameof(config.RoutingKey)} is null.");

            //result
            return Bind(config.ExchangeName, config.QueueName, config.RoutingKey, config.Arguments);
        }

        /// <summary>
        /// Binds the specified configuration action.
        /// </summary>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit Bind(Action<BindingConfig> configAction)
        {
            var config = new BindingConfig();
            configAction?.Invoke(config);
            Configuration.Bindings.Add(new BindingInfo() { Config = config });


            return Bind(config);
        }

        /// <summary>
        /// Unbinds the specified exchange name.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public FluentRabbit Unbind(string exchangeName, string queueName, string routingKey, IDictionary<string, object> arguments = null)
        {
            RabbitMqClient.Model.ThrowExceptionIfNull<ModelIsNullException>();
            RabbitMqClient.Model.QueueUnbind(queueName, exchangeName, routingKey, arguments);
            return this;
        }

        /// <summary>
        /// Unbinds the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public FluentRabbit Unbind(BindingConfig config)
        {
            //validation
            config.ThrowExceptionIfNull<ArgumentNullException>(nameof(config));

            //result
            return Unbind(config.ExchangeName, config.QueueName, config.RoutingKey, config.Arguments);
        }

        /// <summary>
        /// Unbinds the specified configuration action.
        /// </summary>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit Unbind(Action<BindingConfig> configAction)
        {
            var config = new BindingConfig();
            configAction?.Invoke(config);

            return Unbind(config);
        }

        #endregion

        #region Subscribe
        /// <summary>
        /// Subscribes the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public FluentRabbit Subscribe(string queueName, Action<BasicDeliverEventArgs> callback, SubscribeConfig config)
        {
            config.ThrowExceptionIfNull<ArgumentNullException>(nameof(config));
            RabbitMqClient.Model.ThrowExceptionIfNull<ModelIsNullException>();

            var consumer = new EventingBasicConsumer(RabbitMqClient.Model);
            consumer.Received += (ch, ea) =>
            {
                callback(ea);

                if (!config.AutoAck)
                {
                    RabbitMqClient.Model.BasicAck(ea.DeliveryTag, false);
                }
            };
            RabbitMqClient.Model.BasicConsume(queueName, config.AutoAck, config.ConsumerTag, config.NoLocal, config.Exclusive, config.Arguments, consumer);

            return this;
        }

        /// <summary>
        /// Subscribes the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit Subscribe(string queueName, Action<BasicDeliverEventArgs> callback, Action<SubscribeConfig> configAction = null)
        {
            var config = new SubscribeConfig();
            configAction?.Invoke(config);

            return Subscribe(queueName, callback, config);
        }


        #endregion


        /// <summary>
        /// Fetches the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public FluentRabbit Fetch(string queueName, Action<BasicGetResult> callback, FetchConfig config)
        {
            config.ThrowExceptionIfNull<ArgumentNullException>(nameof(config));
            RabbitMqClient.Model.ThrowExceptionIfNull<ModelIsNullException>();

            var result = RabbitMqClient.Model.BasicGet(queueName, config.AutoAck);
            
            callback?.Invoke(result);

            return this;
        }

        /// <summary>
        /// Fetches the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit Fetch(string queueName, Action<BasicGetResult> callback, Action<FetchConfig> configAction = null)
        {
            var config = new FetchConfig();
            configAction?.Invoke(config);

            return Fetch(queueName, callback, config);
        }

        /// <summary>
        /// Sleeps the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns></returns>
        public FluentRabbit Sleep(int milliseconds = 1000)
        {
            System.Threading.Thread.Sleep(milliseconds);
            return this;
        }


        #region Publish

        /// <summary>
        /// Publishes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public FluentRabbit Publish(byte[] data, PublishConfig config)
        {
            config.ThrowExceptionIfNull<ArgumentNullException>(nameof(config));
            config.Validate();

            RabbitMqClient.Model.ThrowExceptionIfNull<ModelIsNullException>();

            RabbitMqClient.Model.BasicPublish(config.ExchangeName, config.RoutingKey, config.Mandatory, config.BasicProperties, data);
            return this;
        }

        /// <summary>
        /// Publishes the specified payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload">The payload.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public FluentRabbit Publish<T>(T payload, PublishConfig config)
        {
            byte[] body = null;

            if (config.PayloadFormat == PayloadFormat.String)
            {
                payload.ThrowExceptionIf(p => p.GetType() != typeof(string));
                body = Encoding.UTF8.GetBytes(payload.ToString());
            }
            else if (config.PayloadFormat == PayloadFormat.ByteArray)
            {
                var bf = new BinaryFormatter();
                var ms = new MemoryStream();
                bf.Serialize(ms, payload);

                body = ms.ToArray();
            }
            else if (config.PayloadFormat == PayloadFormat.Json)
            {
                using var ms = new MemoryStream();
                var ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(ms, payload);
                body = ms.ToArray();
            }
            else if (config.PayloadFormat == PayloadFormat.Xml)
            {
                throw new NotImplementedException();
            }

            return Publish(body, config);
        }

        /// <summary>
        /// Publishes the specified payload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payload">The payload.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit Publish<T>(T payload, Action<PublishConfig> configAction)
        {
            var config = new PublishConfig();
            configAction.Invoke(config);

            return Publish(payload, config);
        }

        #endregion

        #region Delete Queue
        /// <summary>
        /// Deletes the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public FluentRabbit DeleteQueue(string queueName, DeleteQueueConfig config)
        {
            config.ThrowExceptionIfNull<ArgumentNullException>(nameof(config));
            RabbitMqClient.Model.QueueDelete(queueName, config.IfUnused, config.IfEmpty);

            return this;
        }

        /// <summary>
        /// Deletes the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit DeleteQueue(string queueName, Action<DeleteQueueConfig> configAction = null)
        {
            var config = new DeleteQueueConfig();
            configAction?.Invoke(config);

            return DeleteQueue(queueName, config);
        }
        #endregion

        /// <summary>
        /// Purges the queue.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="resultAction">The result action.</param>
        /// <returns></returns>
        public FluentRabbit PurgeQueue(string queueName, Action<uint> resultAction = null)
        {
            var purgeResult = RabbitMqClient.Model.QueuePurge(queueName); //todo: need to 
            resultAction?.Invoke(purgeResult);
            return this;
        }



        #region Delete Exchange
        /// <summary>
        /// Deletes the exchange.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public FluentRabbit DeleteExchange(string exchangeName, DeleteExchangeConfig config)
        {
            config.ThrowExceptionIfNull<ArgumentNullException>(nameof(config));
            RabbitMqClient.Model.ThrowExceptionIfNull<ModelIsNullException>();

            RabbitMqClient.Model.ExchangeDelete(exchangeName, config.IfUnused);

            return this;
        }

        /// <summary>
        /// Deletes the exchange.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="configAction">The configuration action.</param>
        /// <returns></returns>
        public FluentRabbit DeleteExchange(string exchangeName, Action<DeleteExchangeConfig> configAction = null)
        {
            var config = new DeleteExchangeConfig();
            configAction?.Invoke(config);

            return DeleteExchange(exchangeName, config);
        }
        #endregion


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            RabbitMqClient.Model?.Close();
            RabbitMqClient.Connection?.Close();

            RabbitMqClient.Model?.Dispose();
            RabbitMqClient.Connection?.Dispose();

            RabbitMqClient.Model = null;
            RabbitMqClient.Connection = null;

        }
    }



}
