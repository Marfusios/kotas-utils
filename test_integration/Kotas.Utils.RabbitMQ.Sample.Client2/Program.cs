using System;
using Kotas.Utils.RabbitMQ.Bus;
using Kotas.Utils.RabbitMQ.Config;
using Kotas.Utils.RabbitMQ.Sample.Shared;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Kotas.Utils.RabbitMQ.Sample.Client2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("  ###############################");
            Console.WriteLine("  #  RabbitMQ Client 2 (user)   #");
            Console.WriteLine("  ###############################");
            Console.WriteLine();

            var config = RabbitValues.CONFIG;
            config.ConsumerName = "user";

            var random = new Random(DateTime.Now.Millisecond);
            var userId = random.Next(2, 99);

            var user = new ChatUserPayload()
            {
                Id = userId,
                IsAdmin = false,
                Name = "User " + userId
            };

            Console.WriteLine($"Logged in as {user.Name}");
            Console.WriteLine();

            using (var bus = new RabbitBus(NullLogger<IBus>.Instance, new OptionsWrapper<BusConfig>(config)))
            {
                bus.Init();

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
