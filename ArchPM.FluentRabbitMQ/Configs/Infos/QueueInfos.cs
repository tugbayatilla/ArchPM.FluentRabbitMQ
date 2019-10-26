using System;
using System.Collections.Generic;
using System.Linq;

namespace ArchPM.FluentRabbitMQ.Configs.Infos
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso>
    ///     <cref>System.Collections.Generic.List{ArchPM.FluentRabbitMQ.Configs.Infos.QueueInfo}</cref>
    /// </seealso>
    public class QueueInfos : List<QueueInfo>
    {
        /// <summary>
        /// Adds the specified information.
        /// </summary>
        /// <param name="info">The information.</param>
        public new void Add(QueueInfo info)
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
        public void Add(Action<QueueInfo> addAction) //todo: repeating
        {
            var queue = new QueueInfo();
            addAction(queue);
            Add(queue);
        }

        /// <summary>
        /// Adds the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="config">The configuration.</param>
        public void Add(string queueName, CreateQueueConfig config)
        {
            var queue = new QueueInfo { Name = queueName, Config = config };
            Add(queue);
        }

    }
}
