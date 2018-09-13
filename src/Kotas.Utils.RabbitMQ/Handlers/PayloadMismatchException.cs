using System;

namespace Kotas.Utils.RabbitMQ.Handlers
{
    public class PayloadMismatchException : Exception
    {
        public PayloadMismatchException()
        {
        }

        public PayloadMismatchException(string message) : base(message)
        {
        }

        public PayloadMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
