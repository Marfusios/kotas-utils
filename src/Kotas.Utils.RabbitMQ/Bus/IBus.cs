using System;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Infrastructure;

namespace Kotas.Utils.RabbitMQ.Bus
{
    public interface IBus : IDisposable
    {
        /// <summary>
        /// Initialize Bus, must be called once
        /// </summary>
        IBus Init();

        /// <summary>
        /// Get desired message and call on it Publish/Subscribe method.
        /// </summary>
        TMessage Message<TMessage>() where TMessage : IMessage;

        /// <summary>
        /// Publish fallback. Use _bus.Message<MyDesiredMessage>().Publish(...) when possible
        /// </summary>
        void Publish<TPayload>(IMessage message, TPayload payload) where TPayload : IPayload;

        /// <summary>
        /// Subscribe fallback. Use _bus.Message<MyDesiredMessage>().Subscribe(...) when possible. 
        /// Handler should return true, otherwise the message will be requeued
        /// </summary>
        void Subscribe<TPayload>(IMessage message, Func<IPayloadWrapper<TPayload>, Task> handler, 
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers)
            where TPayload : IPayload;

        /// <summary>
        /// Set or get correlation id factory 
        /// Method is called while publishing message
        /// </summary>
        Func<string> CorrelationIdFactory { get; set; }

    }
}
