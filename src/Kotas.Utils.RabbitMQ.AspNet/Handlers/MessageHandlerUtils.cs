using System;
using Kotas.Utils.RabbitMQ.Bus;
using Kotas.Utils.RabbitMQ.Handlers;
using Kotas.Utils.RabbitMQ.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kotas.Utils.RabbitMQ.AspNet.Handlers
{
    public static class MessageHandlerUtils
    {
        public static void AddMessageHandler<THandler>(this IServiceCollection services)
            where THandler : class, IMessageHandler
        {
            services.AddTransient<THandler>();
        }

        public static void UseMessageHandler<TMessage, THandler>(this IApplicationBuilder app, SubscriptionType subscriptionType)
            where TMessage : IMessage
            where THandler : class, IMessageHandler
        {
            var logger = app.ApplicationServices.GetService<ILogger<RabbitBus>>();
            try
            {
                var bus = (IBus)app.ApplicationServices.GetService(typeof(IBus));
                bus.Message<TMessage>().Subscribe(wrapper =>
                {
                    var handler = app.ApplicationServices.GetService(typeof(THandler)) as THandler;
                    if (handler == null)
                    {
                        throw new InvalidOperationException(
                            $"Can't handle message '{typeof(TMessage).Name}'. " +
                            $"Handler '{typeof(THandler).Name}' is not registered via DI. " +
                            $"Don't forget to call method 'services.AddMessageHandler<{typeof(THandler).Name}>' in Startup");
                    }
                    return handler.Handle(wrapper);
                }, subscriptionType);
                logger.LogInformation($"Subscribed to message '{typeof(TMessage).Name}', handler '{typeof(THandler).Name}', " +
                                      $"type: '{subscriptionType}'");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Subscription to message '{typeof(TMessage).Name}', handler '{typeof(THandler).Name}' failed");
            }
        }
    }
}
