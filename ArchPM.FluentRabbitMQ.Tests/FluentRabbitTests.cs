using System.Linq;
using System.Text;
using ArchPM.FluentRabbitMQ.Configs;
using ArchPM.FluentRabbitMQ.Exceptions;
using ArchPM.FluentRabbitMQ.Tests.Models;
using ArchPM.NetCore.Builders;
using FluentAssertions;
using RabbitMQ.Client.Exceptions;
using Xunit;

namespace ArchPM.FluentRabbitMQ.Tests
{
    public class FluentRabbitTests
    {
        private readonly FluentRabbit _rabbit;

        public FluentRabbitTests()
        {
            _rabbit = new FluentRabbit();
        }

        [Fact]
        public void ExecuteConfigure()
        {
            var queueConfig = new CreateQueueConfig();
            var exchangeConfig = new CreateExchangeConfig();

            var exchange1 = "Exchange1";
            var exchange2 = "Exchange2";
            var queue1 = "Queue1";
            var queue2 = "Queue2";
            var routingKey1 = "RoutingKey1";
            var routingKey2 = "RoutingKey2";

            _rabbit.Configure(
                    p =>
                    {
                        p.Exchanges.Add(exchange1, exchangeConfig);
                        p.Exchanges.Add(exchange2, exchangeConfig);

                        p.Queues.Add(q =>
                            {
                                q.Name = queue1;
                                q.Config = queueConfig;
                            }
                        );
                        p.Queues.Add(queue2, queueConfig);

                        p.Bindings.Add(
                            r =>
                            {
                                r.Config.ExchangeName = exchange1;
                                r.Config.QueueName = queue1;
                                r.Config.RoutingKey = routingKey1;
                            });
                        p.Bindings.Add(
                            r =>
                            {
                                r.Config.ExchangeName = exchange2;
                                r.Config.QueueName = queue2;
                                r.Config.RoutingKey = routingKey2;
                            });

                    }
                )
                .Connect()
                .ConfigureUp()
                .ConfigureDown()
                ;

            _rabbit.Configuration.Exchanges.Should().ContainSingle(p => p.Name == exchange1);
        }

        [Fact]
        public void Configure_Should_be_used_with_predicate()
        {
            _rabbit.Configure(
                p => { p.Exchanges.Add(e => { e.Name = "Exchange1"; }); }
            );

            _rabbit.Configuration
                .Exchanges.Should().ContainSingle(p => p.Name == "Exchange1");
        }

        [Fact]
        public void Connect_should_connect_to_localhost_with_default_configuration()
        {
            _rabbit.Connect();
            _rabbit.RabbitMqClient.Connection.Should().NotBeNull();
            _rabbit.Dispose();
            _rabbit.RabbitMqClient.Model.Should().BeNull();
            _rabbit.RabbitMqClient.Connection.Should().BeNull();
        }

        [Fact]
        public void CreateExchange_should_create_check_exists_delete()
        {
            var exchangeName = "CreateExchange_should_create_check_exists_delete";

            _rabbit
                .Connect()
                .CreateExchange(exchangeName);

            Assert.Throws<OperationInterruptedException>(() =>
            {
                //creating another exchange with same name but different config. 
                //it should throw an exception. it means, exchange is already created
                _rabbit.CreateExchange(exchangeName, p => { p.Durable = false; });
            });

            //because of the exception, we need to reconnect again
            _rabbit
                .Connect()
                .DeleteExchange(exchangeName)
                .Dispose();
        }

        [Fact]
        public void Should_throw_exception_if_connect_is_not_called()
        {
            Assert.Throws<ModelIsNullException>(
                () =>
                {
                    var rabbit = new FluentRabbit();

                    try
                    {
                        rabbit = rabbit.CreateExchange("Exchange1");
                    }
                    finally
                    {
                        rabbit.Dispose();
                    }
                }
            );
        }

        [Fact]
        public void CreateExchange_should_custom_local_config_set_to_global_config()
        {
            var exchangeName = "CreateExchange_should_custom_local_config_set_to_global_config";

            _rabbit.Configure(
                p =>
                {
                    p.Exchanges.Add(e => e.Name = exchangeName);
                });

            //test
            _rabbit.Configuration.Exchanges.First(p => p.Name == exchangeName)
                .Config.AutoDelete.Should()
                .Be(false);

            _rabbit
                .Connect()
                .CreateExchange(exchangeName, p => p.AutoDelete = true)
                //because of the exception, we need to reconnect again
                .DeleteExchange(exchangeName)
                .Dispose();

            //test
            _rabbit.Configuration.Exchanges.First(p => p.Name == exchangeName)
                .Config.AutoDelete.Should()
                .Be(true);
        }

        [Fact]
        public void CreateQueue_should_create_check_exists_delete()
        {
            var queueName = "CreateQueue_should_create_check_exists_delete";

            _rabbit.Connect().CreateQueue(queueName);

            Assert.Throws<OperationInterruptedException>(() =>
            {
                //creating another queue with same name but different config. 
                //it should throw an exception. it means, it is already created
                _rabbit.CreateQueue(queueName, p => { p.Durable = false; });
            });

            //because of the exception, we need to reconnect again
            _rabbit
                .Connect()
                .DeleteQueue(queueName)
                .Dispose();
        }

        [Fact]
        public void CreateQueue_should_custom_local_config_set_to_global_config()
        {
            var queueName = "CreateQueue_should_custom_local_config_set_to_global_config";

            _rabbit.Configure(
                p =>
                {
                    p.Queues.Add(e => e.Name = queueName);
                });

            //test
            _rabbit.Configuration.Queues.First(p => p.Name == queueName)
                .Config.AutoDelete.Should()
                .Be(false);

            _rabbit
                .Connect()
                .CreateQueue(queueName, p => p.AutoDelete = true)
                //because of the exception, we need to reconnect again
                .DeleteQueue(queueName)
                .Dispose();

            //test
            _rabbit.Configuration.Queues.First(p => p.Name == queueName)
                .Config.AutoDelete.Should()
                .Be(true);
        }


        [Fact]
        public void Bind_should_connect_exchange_and_queue()
        {
            var exchangeName = "Exchange_Bind_should_connect_exchange_and_queue";
            var queueName = "Queue_Bind_should_connect_exchange_and_queue";
            var routingKey = "RoutingKey_Bind_should_connect_exchange_and_queue";

            _rabbit
                .Connect()
                .CreateQueue(queueName)
                .CreateExchange(exchangeName)
                .Bind(
                    p =>
                    {
                        p.QueueName = queueName;
                        p.ExchangeName = exchangeName;
                        p.RoutingKey = routingKey;
                    })
                .DeleteQueue(queueName)
                .DeleteExchange(exchangeName)
                .Dispose();
        }

        [Fact]
        public void Unbind_should_disconnect_exchange_and_queue()
        {
            var exchangeName = "Exchange_Unbind_should_disconnect_exchange_and_queue";
            var queueName = "Queue_Unbind_should_disconnect_exchange_and_queue";
            var routingKey = "RoutingKey_Unbind_should_disconnect_exchange_and_queue";

            _rabbit
                .Connect()
                .CreateQueue(queueName)
                .CreateExchange(exchangeName)
                .Bind(
                    p =>
                    {
                        p.QueueName = queueName;
                        p.ExchangeName = exchangeName;
                        p.RoutingKey = routingKey;
                    })
                .Unbind(
                    p =>
                    {
                        p.QueueName = queueName;
                        p.ExchangeName = exchangeName;
                        p.RoutingKey = routingKey;
                    })
                .DeleteQueue(queueName)
                .DeleteExchange(exchangeName)
                .Dispose();
        }


        [Fact]
        public void Bind_should_custom_local_config_set_to_global_config()
        {
            var exchangeName = "Exchange_Bind_should_custom_local_config_set_to_global_config";
            var queueName = "Queue_Bind_should_custom_local_config_set_to_global_config";
            var routingKey = "RoutingKey_Bind_should_custom_local_config_set_to_global_config";

            //configure
            _rabbit.Configure(
                p =>
                {
                    p.Bindings.Add(b =>
                    {
                        b.Config.QueueName = queueName;
                        b.Config.ExchangeName = exchangeName;
                        b.Config.RoutingKey = routingKey;
                    });
                });

            //test
            _rabbit.Configuration.Bindings.First(p => p.Config.QueueName == queueName)
                .Config.QueueName.Should()
                .Be(queueName);

            _rabbit
                .Connect()
                .CreateQueue(queueName)
                .CreateExchange(exchangeName)
                .Bind(p =>
                {
                    p.RoutingKey = "newRoutingKey";
                    p.QueueName = queueName;
                    p.ExchangeName = exchangeName;
                })
                .DeleteQueue(queueName)
                .DeleteExchange(exchangeName);

            _rabbit.Configuration.Bindings.Should().Contain(p => p.Config.RoutingKey == "newRoutingKey");

            _rabbit.Dispose();
        }

        [Fact]
        public void Publish_should_send_string_message_to_queue()
        {
            var exchangeName = "Exchange_Publish_should_send_string_message_to_queue";
            var queueName = "Queue_Publish_should_send_string_message_to_queue";
            var routingKey = "RoutingKey_Publish_should_send_string_message_to_queue";

            _rabbit
                .Connect()
                .CreateQueue(queueName)
                .CreateExchange(exchangeName)
                .Bind(
                    p =>
                    {
                        p.RoutingKey = routingKey;
                        p.QueueName = queueName;
                        p.ExchangeName = exchangeName;
                    }
                )
                .Publish("this is string",
                    p =>
                    {
                        p.ExchangeName = exchangeName;
                        p.RoutingKey = routingKey;
                        p.PayloadFormat = PayloadFormat.String;
                    })
                .DeleteQueue(queueName)
                .DeleteExchange(exchangeName)
                .Dispose();
        }

        [Fact]
        public void Publish_should_send_object_as_byte_array_message_to_queue()
        {
            var exchangeName = "Exchange_Publish_should_send_object_as_byte_array_message_to_queue";
            var queueName = "Queue_Publish_should_send_object_as_byte_array_message_to_queue";
            var routingKey = "RoutingKey_Publish_should_send_object_as_byte_array_message_to_queue";

            var sampleClass = SampleBuilder.Create<SerializableSampleClass>();

            _rabbit
                .Connect()
                .CreateQueue(queueName)
                .CreateExchange(exchangeName)
                .Bind(
                    p =>
                    {
                        p.RoutingKey = routingKey;
                        p.QueueName = queueName;
                        p.ExchangeName = exchangeName;
                    }
                )
                .Publish(sampleClass,
                    p =>
                    {
                        p.ExchangeName = exchangeName;
                        p.RoutingKey = routingKey;
                        p.PayloadFormat = PayloadFormat.ByteArray;
                    })
                .DeleteQueue(queueName)
                .DeleteExchange(exchangeName)
                .Dispose();
        }

        [Fact]
        public void Publish_should_send_object_as_json_message_to_queue()
        {
            var exchangeName = "Exchange_Publish_should_send_object_as_json_message_to_queue";
            var queueName = "Queue_Publish_should_send_object_as_json_message_to_queue";
            var routingKey = "RoutingKey_Publish_should_send_object_as_json_message_to_queue";

            var sampleClass = SampleBuilder.Create<SerializableSampleClass>();

            _rabbit
                .Connect()
                .CreateQueue(queueName)
                .CreateExchange(exchangeName)
                .Bind(
                    p =>
                    {
                        p.RoutingKey = routingKey;
                        p.QueueName = queueName;
                        p.ExchangeName = exchangeName;
                    }
                )
                .Publish(sampleClass,
                    p =>
                    {
                        p.ExchangeName = exchangeName;
                        p.RoutingKey = routingKey;
                        p.PayloadFormat = PayloadFormat.Json;
                    })
                .DeleteQueue(queueName)
                .DeleteExchange(exchangeName)
                .Dispose();
        }

        [Fact]
        public void Subscribe_should_read_published_string()
        {
            var exchangeName = "Exchange_Publish_should_send_object_as_json_message_to_queue";
            var queueName = "Queue_Publish_should_send_object_as_json_message_to_queue";
            var routingKey = "RoutingKey_Publish_should_send_object_as_json_message_to_queue";

            var result = "";
            _rabbit
                .Connect()
                .CreateQueue(queueName)
                .CreateExchange(exchangeName)
                .Bind(
                    p =>
                    {
                        p.RoutingKey = routingKey;
                        p.QueueName = queueName;
                        p.ExchangeName = exchangeName;
                    }
                )
                .Publish("test",
                    p =>
                    {
                        p.ExchangeName = exchangeName;
                        p.RoutingKey = routingKey;
                        p.PayloadFormat = PayloadFormat.String;
                    })
                .Subscribe(queueName,
                    p => { result = Encoding.UTF8.GetString(p); })
                .DeleteQueue(queueName)
                .DeleteExchange(exchangeName)
                .Dispose();

            result.Should().Be("test");
        }
    }
}
