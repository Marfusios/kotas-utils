using Kotas.Utils.RabbitMQ.Infrastructure;

namespace Kotas.Utils.RabbitMQ.Messages.Example
{
    public class ExamplePayload : IPayload
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public ExampleModel Model { get; set; }

    }
}