using System;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Bus;

namespace Kotas.Utils.RabbitMQ.Infrastructure
{
    public abstract class Message<TPayload> : IMessage where TPayload : IPayload
    {
        private readonly IBus _bus;

        protected Message(IBus bus)
        {
            _bus = bus;
        }

        public abstract string RoutingKey { get; }

        /// <summary>
        /// Publish the message with selected payload
        /// </summary>
        /// <param name="payload"></param>
        public void Publish(TPayload payload)
        {
            _bus.Publish(this, payload);
        }

        /// <summary>
        /// Subscribe to the message. 
        /// Handler should return true, otherwise the message will be requeued.
        /// </summary>
        public void Subscribe(Func<IPayloadWrapper<TPayload>, Task> handler, 
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers)
        {
            _bus.Subscribe(this, handler, type);
        }

        public void Subscribe(Func<object, Task> handler, 
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers)
        {
            _bus.Subscribe<TPayload>(this, handler, type);
        }

        protected string GenerateRoutingKey(params string[] parts)
        {
            return string.Join(".", parts);
        }
    }
}
