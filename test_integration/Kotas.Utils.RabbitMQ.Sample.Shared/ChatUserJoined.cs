using Kotas.Utils.RabbitMQ.Bus;
using Kotas.Utils.RabbitMQ.Infrastructure;

namespace Kotas.Utils.RabbitMQ.Sample.Shared
{
    public class ChatUserJoined : Message<ChatUserPayload>
    {
        public ChatUserJoined(IBus bus) : base(bus)
        {
        }

        public override string RoutingKey => GenerateRoutingKey("chat", "user", "joined");
    }

    public class ChatUserPayload : IPayload
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
    }
}
