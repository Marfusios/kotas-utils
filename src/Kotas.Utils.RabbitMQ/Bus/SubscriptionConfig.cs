using System;
using System.Collections.Generic;
using System.Text;

namespace Kotas.Utils.RabbitMQ.Bus
{
    /// <summary>
    /// Configuration per single subscription
    /// </summary>
    public class SubscriptionConfig
    {
        /// <summary>
        /// How many messages per this queue should be processed in parallel.
        /// Default is 1 (from global BusConfig - appsettings.json). 
        /// </summary>
        public int? ConcurrencyLimit { get; set; }

        /// <summary>
        /// Automatically create and bind dead letter queue.
        /// It will be named like 'name-of-the-queue.dead'. 
        /// All rejected messages will fall down there. 
        /// </summary>
        public bool? CreateDeadLetterQueue { get; set; }

        /// <summary>
        /// Additional queue arguments used on creation
        /// </summary>
        public IDictionary<string, object> QueueArguments { get; set; }
    }
}
