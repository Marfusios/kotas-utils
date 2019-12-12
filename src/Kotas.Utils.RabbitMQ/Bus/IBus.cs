using System;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Handlers;
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
        /// Handler should return result, in case of exception the message will be re-queued
        /// </summary>
        void Subscribe<TPayload>(IMessage message, Func<IPayloadWrapper<TPayload>, Task<HandleResult>> handler, 
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers, SubscriptionConfig configuration = null)
            where TPayload : IPayload;

        /// <summary>
        /// Get first message from the target queue
        /// </summary>
        IPayloadWrapper<TPayload> Get<TPayload>(IMessage message, SubscriptionType type = SubscriptionType.SharedBetweenConsumers) 
            where TPayload : IPayload;

        /// <summary>
        /// Get all messages from the target queue
        /// </summary>
        IPayloadWrapper<TPayload>[] GetAll<TPayload>(IMessage message, SubscriptionType type = SubscriptionType.SharedBetweenConsumers)
            where TPayload : IPayload;

        /// <summary>
        /// Set or get correlation id factory 
        /// Method is called while publishing message
        /// </summary>
        Func<string> CorrelationIdFactory { get; set; }

    }
}
