using System;
using System.Linq;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Infrastructure;

namespace Kotas.Utils.RabbitMQ.Handlers
{
    public abstract class MessageHandler<TPayload> : IMessageHandler
        where TPayload: IPayload
    {
        /// <summary>
        /// Handle subscribed message
        /// </summary>
        public Task<HandleResult> Handle(object data)
        {
            var wrapper = data as IPayloadWrapper<TPayload>;
            if(wrapper == null)
                throw new PayloadMismatchException($"Mismatch of payload type between message and handler. " +
                                                     $"Handler: <{GetType()}>  |  CHANGE <{typeof(TPayload)}> TO <{GetInnerGeneric(data)}>");
            return Handle(wrapper);
        }

        /// <summary>
        /// Handle subscribed message
        /// </summary>
        public abstract Task<HandleResult> Handle(IPayloadWrapper<TPayload> wrapper);

        private static Type GetInnerGeneric(object data)
        {
            var type = data?.GetType();
            if (type != null && type.GenericTypeArguments.Any())
                return type.GenericTypeArguments[0];
            return type;
        }
    }
}
