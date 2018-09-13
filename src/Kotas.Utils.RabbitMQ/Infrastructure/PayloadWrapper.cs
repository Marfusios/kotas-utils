using System;

namespace Kotas.Utils.RabbitMQ.Infrastructure
{
    internal class PayloadWrapper<TPayload> : IPayloadWrapper<TPayload> where TPayload : IPayload
    {
        public PayloadWrapper(TPayload payload, DateTime created, string correlationId)
        {
            Payload = payload;
            Created = created;
            CorrelationId = correlationId;
        }

        public TPayload Payload { get; }

        public DateTime Created { get; }

        public string CorrelationId { get; }
    }
}
