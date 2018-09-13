using System.Threading.Tasks;

namespace Kotas.Utils.RabbitMQ.Handlers
{
    public interface IMessageHandler
    {
        Task Handle(object data);
    }
}
