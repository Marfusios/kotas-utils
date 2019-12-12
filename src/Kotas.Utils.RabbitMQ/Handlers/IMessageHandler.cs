using System.Threading.Tasks;

namespace Kotas.Utils.RabbitMQ.Handlers
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Handle subscribed message
        /// </summary>
        Task<HandleResult> Handle(object data);
    }
}
