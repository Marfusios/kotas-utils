using System;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Bus;
using Kotas.Utils.RabbitMQ.Config;

namespace Kotas.Utils.RabbitMQ.Sample.Shared
{
    public static class RabbitValues
    {
        public static readonly BusConfig CONFIG = new BusConfig()
        {
            HostUri = "amqp://",
            UserName = "",
            Password = ""
        };

        public static void ChatJoinedHandler(RabbitBus bus)
        {
            bus.Message<ChatUserJoined>().Subscribe(wrapper =>
            {
                var payload = wrapper.Payload;
                var adminText = payload.IsAdmin ? "*" : string.Empty;
                Console.WriteLine($"[{adminText}{payload.Name}] joined");

                return Task.FromResult(true);
            }, SubscriptionType.SharedBetweenConsumers);
        }

        public static void ChatHandler(RabbitBus bus)
        {
            bus.Message<ChatMessageInserted>().Subscribe(wrapper =>
            {
                var payload = wrapper.Payload;
                if (payload.IsProtected)
                {
                    Console.WriteLine($"[{payload.User.Name}] > *** protected ***");
                }
                else
                {
                    Console.WriteLine($"[{payload.User.Name}] > {payload.Plaintext}");
                }

                return Task.FromResult(true);
            }, SubscriptionType.PerConsumer);
        }

        public static void DeletePrevConsoleLine()
        {
            if (Console.CursorTop == 0) return;
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
    }
}
