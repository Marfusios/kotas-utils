using Kotas.Utils.RabbitMQ.Bus;
using Kotas.Utils.RabbitMQ.Infrastructure;

namespace Kotas.Utils.RabbitMQ.Messages.Example
{
    public class ExampleMessage : Message<ExamplePayload>
    {
        public ExampleMessage(IBus bus) : base(bus)
        {
        }

        public override string RoutingKey => GenerateRoutingKey("example", "message", "changed");
    }
}