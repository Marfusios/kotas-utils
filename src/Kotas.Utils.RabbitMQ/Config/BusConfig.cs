namespace Kotas.Utils.RabbitMQ.Config
{
    public class BusConfig
    {
        public const string DEFAULT_USER_NAME = "guest";
        public const string DEFAULT_PASSWORD = "guest";

        /// <summary>
        /// RabbitMQ server url
        /// </summary>
        public string HostUri { get; set; }

        /// <summary>
        /// Client/consumer name (to be displayed in the RabbitMQ GUI)
        /// </summary>
        public string ConsumerName { get; set; }

        /// <summary>
        /// Heartbeat interval in seconds
        /// </summary>
        public ushort HeartbeatInterval { get; set; } = 30;

        /// <summary>
        /// Whenever it should reconnect on network failure
        /// </summary>
        public bool AutomaticRecovery { get; set; } = true;
        /// <summary>
        /// Client username
        /// </summary>
        public string UserName { get; set; } = DEFAULT_USER_NAME;

        /// <summary>
        /// Client password
        /// </summary>
        public string Password { get; set; } = DEFAULT_PASSWORD;

        /// <summary>
        /// How many messages per one queue should be processed in parallel.
        /// Default is 1. 
        /// </summary>
        public int ConcurrencyLimit { get; set; } = 1;

        /// <summary>
        /// Automatically create and bind dead letter queue for every queue.
        /// It will be named like 'name-of-the-queue.dead'. 
        /// All rejected messages will fall down there. 
        /// </summary>
        public bool? CreateDeadLetterQueue { get; set; }
    }
}
