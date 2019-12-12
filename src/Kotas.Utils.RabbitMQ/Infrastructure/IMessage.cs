using System;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Bus;
using Kotas.Utils.RabbitMQ.Handlers;

namespace Kotas.Utils.RabbitMQ.Infrastructure
{
    public interface IMessage
    {
        string RoutingKey { get; }

        void Subscribe(Func<object, Task<HandleResult>> handler, 
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers,
            SubscriptionConfig configuration = null);
    }
}