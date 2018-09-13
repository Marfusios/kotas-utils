using System;

namespace Kotas.Utils.RabbitMQ.Messages.Example
{
    public class ExampleModel
    {
        public int Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Data { get; set; }
    }
}