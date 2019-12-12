using System;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Bus;
using Kotas.Utils.RabbitMQ.Config;
using Kotas.Utils.RabbitMQ.Handlers;

namespace Kotas.Utils.RabbitMQ.Sample.Shared
{
    public static class RabbitValues
    {
        public static readonly BusConfig CONFIG = new BusConfig()
        {
            HostUri = "amqp://ttrjrcuh:pyvHc1UIOKfAiI3TAWvPA5Dqd-LZkszM@stingray.rmq.cloudamqp.com/ttrjrcuh",
            UserName = "ttrjrcuh",
            Password = "pyvHc1UIOKfAiI3TAWvPA5Dqd-LZkszM"
        };

        public static void ChatJoinedHandler(RabbitBus bus)
        {
            var messages = bus.Message<ChatUserJoined>().GetAll();
            Console.WriteLine($"[*System] {messages.Length} users joined in the meantime");

            bus.Message<ChatUserJoined>().Subscribe(wrapper =>
            {
                var payload = wrapper.Payload;
                var adminText = payload.IsAdmin ? "*" : string.Empty;
                Console.WriteLine($"[{adminText}{payload.Name}] joined");

                return Task.FromResult(HandleResult.Ok);
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

                return Task.FromResult(HandleResult.Ok);
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
