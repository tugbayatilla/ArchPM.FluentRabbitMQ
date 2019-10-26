using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchPM.FluentRabbitMQ.Configs.Infos
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso>
    ///     <cref>System.Collections.Generic.List{ArchPM.FluentRabbitMQ.Configs.Infos.ExchangeInfo}</cref>
    /// </seealso>
    public class ExchangeInfos : List<ExchangeInfo>
    {
        /// <summary>
        /// Adds the specified information. if same item exists, overrides on existing one.
        /// </summary>
        /// <param name="info">The information.</param>
        public new void Add(ExchangeInfo info)
        {
            var item = this.FirstOrDefault(p => p.Name == info.Name);
            if (item != null)
            {
                Remove(item);
            }

            base.Add(info);
        }

        /// <summary>
        /// Adds the specified add action.
        /// </summary>
        /// <param name="addAction">The add action.</param>
        public void Add(Action<ExchangeInfo> addAction)
        {
            var exchange = new ExchangeInfo();
            addAction(exchange);
            Add(exchange);
        }

        /// <summary>
        /// Adds the specified exchange name.
        /// </summary>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="config">The configuration.</param>
        public void Add(string exchangeName, CreateExchangeConfig config)
        {
            var exchange = new ExchangeInfo() { Name = exchangeName, Config = config };
            Add(exchange);
        }
    }
}
