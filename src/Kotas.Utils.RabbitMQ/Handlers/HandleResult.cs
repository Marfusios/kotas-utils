using System;
using System.Collections.Generic;
using System.Text;

namespace Kotas.Utils.RabbitMQ.Handlers
{
    /// <summary>
    /// Message handling result
    /// </summary>
    public class HandleResult
    {
        /// <inheritdoc />
        public HandleResult(HandleType type)
        {
            Type = type;
        }

        /// <summary>
        /// Handle type
        /// </summary>
        public HandleType Type { get; }


        /// <summary>
        /// Remove message from queue
        /// </summary>
        public static HandleResult Ok => new HandleResult(HandleType.Ok);

        /// <summary>
        /// Requeue message
        /// </summary>
        public static HandleResult Requeue => new HandleResult(HandleType.Requeue);

        /// <summary>
        /// Remove message from queue and put it into dead-letter
        /// </summary>
        public static HandleResult Reject => new HandleResult(HandleType.Reject);
    }
}
