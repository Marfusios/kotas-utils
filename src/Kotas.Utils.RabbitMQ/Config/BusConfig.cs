namespace Kotas.Utils.RabbitMQ.Config
{
    public class BusConfig
    {
        public const string DEFAULT_USER_NAME = "guest";
        public const string DEFAULT_PASSWORD = "guest";

        public string HostUri { get; set; }

        public string ConsumerName { get; set; }

        public ushort HeartbeatInterval { get; set; } = 30;

        public bool AutomaticRecovery { get; set; } = true;

        public string UserName { get; set; } = DEFAULT_USER_NAME;

        public string Password { get; set; } = DEFAULT_PASSWORD;
    }
}
