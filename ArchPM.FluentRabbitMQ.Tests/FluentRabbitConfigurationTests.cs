using ArchPM.FluentRabbitMQ.Configs;
using FluentAssertions;
using Xunit;

namespace ArchPM.FluentRabbitMQ.Tests
{
    public class FluentRabbitConfigurationTests
    {
        [Fact]
        public void AddExchange_Should_add_exchange_and_set_new_values()
        {
            var config = new FluentRabbitConfiguration();
            config.Exchanges.Add(
                p => { p.Name = "Exchange1"; });

            config.Exchanges.Should().ContainSingle(p => p.Name == "Exchange1");
        }

        [Fact]
        public void AddExchange_Should_override_same_names()
        {
            var config = new FluentRabbitConfiguration();
            config.Exchanges.Add(
                p => { p.Name = "Exchange1";
                    p.Config = new CreateExchangeConfig();
                });
            config.Exchanges.Add(
                p => { p.Name = "Exchange1"; p.Config = new CreateExchangeConfig(); });

            config.Exchanges.Should().ContainSingle(p => p.Name == "Exchange1");
        }
    }
}
