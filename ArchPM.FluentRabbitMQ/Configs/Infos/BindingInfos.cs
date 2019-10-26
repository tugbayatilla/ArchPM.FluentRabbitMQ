using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchPM.FluentRabbitMQ.Configs.Infos
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{ArchPM.FluentRabbitMQ.Configs.Infos.BindingInfo}" />
    public class BindingInfos : List<BindingInfo>
    {
        /// <summary>
        /// Adds the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        public new void Add(BindingInfo info)
        {
            var item = this.FirstOrDefault(
                p => p.Config.ExchangeName == info.Config.ExchangeName
                    && p.Config.QueueName == info.Config.QueueName
                    && p.Config.RoutingKey == info.Config.RoutingKey);

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
        public void Add(Action<BindingInfo> addAction) //todo: repeating
        {
            var relation = new BindingInfo();
            addAction(relation);
            Add(relation);
        }

    }
}
