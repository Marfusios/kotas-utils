using System;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Bus;
using Kotas.Utils.RabbitMQ.Handlers;

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
        /// Handler should return result, in case of exception the message will be re-queued
        /// </summary>
        public void Subscribe(Func<IPayloadWrapper<TPayload>, Task<HandleResult>> handler, 
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers, SubscriptionConfig configuration = null)
        {
            _bus.Subscribe(this, handler, type, configuration);
        }

        public void Subscribe(Func<object, Task<HandleResult>> handler, 
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers, SubscriptionConfig configuration = null)
        {
            _bus.Subscribe<TPayload>(this, handler, type, configuration);
        }

        /// <summary>
        /// Get first message from the target queue
        /// </summary>
        public IPayloadWrapper<TPayload> Get(SubscriptionType type = SubscriptionType.SharedBetweenConsumers)
        {
            return _bus.Get<TPayload>(this, type);
        }

        /// <summary>
        /// Get all messages from the target queue
        /// </summary>
        public IPayloadWrapper<TPayload>[] GetAll( SubscriptionType type = SubscriptionType.SharedBetweenConsumers)
        {
            return _bus.GetAll<TPayload>(this, type);
        }

        protected string GenerateRoutingKey(params string[] parts)
        {
            return string.Join(".", parts);
        }
    }
}
