using System;
using Kotas.Utils.RabbitMQ.Bus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Kotas.Utils.RabbitMQ.AspNet.Extensions
{
    public static class Extensions
    {
        public static void AddRabbitMQ(
            this IServiceCollection collection, 
            string uri, 
            string consumerName, 
            ushort heartbeat, 
            bool reconnecting,
            string userName,
            string password)
        {
            collection.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Must be singleton (don't try to change, it hurts)

            collection.AddSingleton<IBus, RabbitBus>();          
        }

        public static void UseRabbitMQ(this IApplicationBuilder app)
        {
            var bus = app.ApplicationServices.GetService<IBus>();
            if (bus == null)
            {
                throw new Exception("RabbitMQ IBus is not registered yet. Call services.AddRabbitMQ(...) first.");
            }

            if (bus.CorrelationIdFactory == null)
            {
                bus.CorrelationIdFactory = CreateCorrelationIdFactory(app.ApplicationServices);
            }

            bus.Init();

            var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            lifetime.ApplicationStopping.Register(OnShutdown, bus);
        }

        private static Func<string> CreateCorrelationIdFactory(IServiceProvider provider)
        {
            return () =>
            {
                var context = provider.GetService<IHttpContextAccessor>()?.HttpContext;
                return context?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
            };
        }

        private static string ParseUri(string uri)
        {
            var uriSafe = uri ?? string.Empty;
            return !uriSafe.Contains("://") ? $"amqp://{uri}" : uri;
        }

        private static void OnShutdown(object bus)
        {
            (bus as IBus)?.Dispose();
        }
    }
}
