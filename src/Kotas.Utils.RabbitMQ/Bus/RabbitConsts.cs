namespace Kotas.Utils.RabbitMQ.Bus
{
    public static class RabbitConsts
    {
        public static string ExchangeName = "app.main.exchange";
        public static string QueuePrefix = "app";
        public static string QueueConsumerPrefix = "consumer";

        public static string GenerateQueueName(params string[] parts)
        {
            return string.Join(".", parts);
        }
    }
}