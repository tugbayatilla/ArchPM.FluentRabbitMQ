using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using ArchPM.FluentRabbitMQ.Configs;
using ArchPM.FluentRabbitMQ.Exceptions;
using ArchPM.FluentRabbitMQ.Tests.Models;
using ArchPM.NetCore.Builders;
using ArchPM.NetCore.Extensions;
using FluentAssertions;
using RabbitMQ.Client.Exceptions;
using Xunit;
using Xunit.Abstractions;

namespace ArchPM.FluentRabbitMQ.Tests
{
    public class FluentRabbitTests
    {
        private readonly FluentRabbit _rabbit;

        public FluentRabbitTests(ITestOutputHelper output)
        {
            _rabbit = new FluentRabbit();
            _rabbit.Trace(
                p =>
                {
                    Debug.WriteLine($"{p.Method.Name} {p.Message}");
                    output.WriteLine($"{p.Time:yy-MM-dd.hh:mm:ss:fffff}: {p.Method.Name} {p.Message}");
                    if (p.IsException)
                    {
                        Debug.WriteLine($"{p.Exception.GetAllMessages()}");
                        output.WriteLine($"{p.Exception.GetAllMessages()}");
                    }
                });
        }

        private FluentRabbitConfiguration GetFluentRabbitConfiguration(string methodName = null)
        {
            methodName = methodName ?? MethodBase.GetCurrentMethod().Name;

            var exchangeName = $"Exchange_{methodName}_{Guid.NewGuid()}";
            var queueName = $"Queue_{methodName}_{Guid.NewGuid()}";
            var routingKey = $"RoutingKey_{methodName}_{Guid.NewGuid()}";

            var config = new FluentRabbitConfiguration();
            config.Exchanges.Add(
                p => { p.Name = exchangeName; });
            config.Queues.Add(p => { p.Name = queueName; });
            config.Bindings.Add(
                p =>
                {
                    p.Config.ExchangeName = exchangeName;
                    p.Config.QueueName = queueName;
                    p.Config.RoutingKey = routingKey;
                });

            return config;
        }


        [Fact]
        public void ExecuteConfigure()
        {
            var queueConfig = new CreateQueueConfig();
            var exchangeConfig = new CreateExchangeConfig();

            var exchange1 = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var exchange2 = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var queue1 = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}"; 
            var queue2 = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}"; 
            var routingKey1 = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var routingKey2 = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

            _rabbit.Configure(
                p => { p.Exchanges.Add(e => { e.Name = exchangeName; }); }
            );

            _rabbit.Configuration
                .Exchanges.Should().ContainSingle(p => p.Name == exchangeName);
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
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

            Assert.Throws<ModelIsNullException>(
                () =>
                {
                    var rabbit = new FluentRabbit();

                    try
                    {
                        rabbit = rabbit.CreateExchange(exchangeName);
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
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var routingKey = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var routingKey = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";


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
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var routingKey = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var newRoutingKey = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
                    p.RoutingKey = newRoutingKey;
                    p.QueueName = queueName;
                    p.ExchangeName = exchangeName;
                })
                .DeleteQueue(queueName)
                .DeleteExchange(exchangeName);

            _rabbit.Configuration.Bindings.Should().Contain(p => p.Config.RoutingKey == newRoutingKey);

            _rabbit.Dispose();
        }

        [Fact]
        public void Publish_should_send_string_message_to_queue()
        {
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var routingKey = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
                .Publish("sampleData",
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
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var routingKey = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var routingKey = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
        public void Subscribe_should_read_published_string()
        {
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var routingKey = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

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
                .Subscribe(queueName,
                    p => { result = Encoding.UTF8.GetString(p.Body); })
                .Publish("test data",
                    p =>
                    {
                        p.ExchangeName = exchangeName;
                        p.RoutingKey = routingKey;
                        p.PayloadFormat = PayloadFormat.String;
                    })
                .DeleteExchange(exchangeName)
                .DeleteQueue(queueName)
                .Dispose();

            result.Should().Be("test data");
        }

        [Fact]
        public void Fetch_should_run_without_calling_delete_queue_method()
        {
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var routingKey = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

            var result = "";
            _rabbit.Connect()
                .CreateExchange(exchangeName)
                .CreateQueue(queueName)
                .PurgeQueue(queueName)
                .Bind(exchangeName, queueName, routingKey)
                .Publish(
                    "publish this string",
                    p =>
                    {
                        p.PayloadFormat = PayloadFormat.String;
                        p.ExchangeName = exchangeName;
                        p.RoutingKey = routingKey;
                    }
                )
                .Fetch(queueName,
                    (p) =>
                {
                    result = Encoding.UTF8.GetString(p.Body);
                })
                .DeleteExchange(exchangeName)
                .DeleteQueue(queueName)
                .Dispose();

            result.Should().Be("publish this string");
        }


        [Fact]
        public void Subscribe_should_run_without_calling_delete_queue_method()
        {
            var exchangeName = $"Exchange_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var queueName = $"Queue_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";
            var routingKey = $"RoutingKey_{MethodBase.GetCurrentMethod().Name}_{Guid.NewGuid()}";

            var result = "";
            _rabbit
                .Connect()
                .CreateQueue(queueName)
                .PurgeQueue(queueName)
                .CreateExchange(exchangeName)
                .Bind(
                    p =>
                    {
                        p.RoutingKey = routingKey;
                        p.QueueName = queueName;
                        p.ExchangeName = exchangeName;
                    }
                )
                .Subscribe(queueName,
                    p => { result = Encoding.UTF8.GetString(p.Body); })
                .Publish("test data",
                    p =>
                    {
                        p.ExchangeName = exchangeName;
                        p.RoutingKey = routingKey;
                        p.PayloadFormat = PayloadFormat.String;
                    })
                .WaitUntil(() => !string.IsNullOrWhiteSpace(result), 2000)
                ;

            result.Should().Be("test data");

            _rabbit.DeleteQueue(queueName).DeleteExchange(exchangeName).Dispose();
        }

        [Fact]
        public void Configure_should_work_with_concrete_config()
        {
            var config = GetFluentRabbitConfiguration();
            var data = Guid.NewGuid().ToString();
            var result = "";

            _rabbit
                .Connect()
                .Configure(config)
                .ConfigureUp()
                .Publish(data,
                    p =>
                    {
                        p.PayloadFormat = PayloadFormat.String;
                        p.ExchangeName = config.Bindings.First().Config.ExchangeName;
                        p.RoutingKey = config.Bindings.First().Config.RoutingKey;
                    })
                .Fetch(config.Bindings.First().Config.QueueName,
                    p => { result = Encoding.UTF8.GetString(p.Body); })
                .ConfigureDown()
                .Dispose()
                ;

            result.Should().Be(data);
        }

    }
}
