using Kotas.Utils.RabbitMQ.Bus;
using Kotas.Utils.RabbitMQ.Infrastructure;

namespace Kotas.Utils.RabbitMQ.Sample.Shared
{
    public class ChatMessageInserted : Message<ChatMessagePayload>
    {
        public ChatMessageInserted(IBus bus) : base(bus)
        {
        }

        public override string RoutingKey => GenerateRoutingKey("chat", "message", "inserted");
    }

    public class ChatMessagePayload : IPayload
    {
        public ChatUserPayload User { get; set; }
        public string Plaintext { get; set; }
        public bool IsProtected { get; set; }
    }
}
