using Kotas.Utils.RabbitMQ.Infrastructure;

namespace Kotas.Utils.RabbitMQ.Messages
{
    public class PayloadGeneric : IPayload
    {
        public PayloadGeneric(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
