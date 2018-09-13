using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Kotas.Utils.RabbitMQ.Bus
{
    public class BusFallback : IBus
    {
        private readonly ILogger<IBus> _logger;

        public BusFallback(ILogger<IBus> logger)
        {
            _logger = logger;
        }

        public IBus Init()
        {
            LogMessage($"Initializing bus");
            return this;
        }

        public TMessage Message<TMessage>() where TMessage : IMessage
        {
            LogMessage($"Creating message '{typeof(TMessage).Name}'");
            var message = (TMessage)Activator.CreateInstance(typeof(TMessage), this);
            return message;
        }

        public void Publish<TPayload>(IMessage message, TPayload payload) where TPayload : IPayload
        {
            LogMessage($"Publishing message '{message?.GetType().Name}'");
        }

        public void Subscribe<TPayload>(IMessage message, Func<IPayloadWrapper<TPayload>, Task> handler, 
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers) where TPayload : IPayload
        {
            LogMessage($"Subscribing to message '{message?.GetType().Name}', type: {type}");
        }

        public Func<string> CorrelationIdFactory { get; set; }

        public void Dispose()
        {
            LogMessage("Disposing");
        }

        private void LogMessage(string msg)
        {
            var message = $"[Bus fallback] {msg}";
            if (_logger != null)
                _logger?.LogWarning(message);
            else
                Debug.WriteLine(message);
        }
    }
}
