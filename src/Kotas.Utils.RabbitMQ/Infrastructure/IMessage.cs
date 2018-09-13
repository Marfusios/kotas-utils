using System;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Bus;

namespace Kotas.Utils.RabbitMQ.Infrastructure
{
    public interface IMessage
    {
        string RoutingKey { get; }

        void Subscribe(Func<object, Task> handler, 
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers);
    }
}