using System;
using Kotas.Utils.RabbitMQ.Bus;
using Kotas.Utils.RabbitMQ.Config;
using Kotas.Utils.RabbitMQ.Sample.Shared;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Kotas.Utils.RabbitMQ.Sample.Client1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("  ###############################");
            Console.WriteLine("  #  RabbitMQ Client 1 (admin)  #");
            Console.WriteLine("  ###############################");
            Console.WriteLine();

            var config = RabbitValues.CONFIG;
            config.ConsumerName = "admin";

            var user = new ChatUserPayload()
            {
                Id = 1,
                IsAdmin = true,
                Name = "System Admin"
            };

            Console.WriteLine($"Logged in as {user.Name}");
            Console.WriteLine();

            using (var bus = new RabbitBus(NullLogger<IBus>.Instance, new OptionsWrapper<BusConfig>(config)))
            {
                bus.Init();

                RabbitValues.ChatJoinedHandler(bus);
                RabbitValues.ChatHandler(bus);

                bus.Message<ChatUserJoined>().Publish(user);

                while (true)
                {
                    var msg = Console.ReadLine();
                    RabbitValues.DeletePrevConsoleLine();

                    bus.Message<ChatMessageInserted>().Publish(new ChatMessagePayload()
                    {
                        User = user,
                        IsProtected = msg == "secret",
                        Plaintext = msg
                    });
                }
            }
        }
    }
}
