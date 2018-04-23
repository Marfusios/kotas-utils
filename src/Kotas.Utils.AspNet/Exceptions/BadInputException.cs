using System;

namespace Kotas.Utils.AspNet.Exceptions
{
    public class BadInputException: Exception
    {
        public BadInputException()
        {
        }

        public BadInputException(string message) : base(message)
        {
        }

        public BadInputException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
