using System;

namespace Kotas.Utils.RabbitMQ.Infrastructure
{
    public interface IPayloadWrapper<out TPayload> where TPayload : IPayload
    {
        TPayload Payload { get; }

        DateTime Created { get; }

        string CorrelationId { get; }
    }
}