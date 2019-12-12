using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Kotas.Utils.RabbitMQ.Config;
using Kotas.Utils.RabbitMQ.Handlers;
using Kotas.Utils.RabbitMQ.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;

namespace Kotas.Utils.RabbitMQ.Bus
{
    public class RabbitBus : IBus
    {
        private static readonly string INSTANCE_IDENTIFICATION = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).ToLower();

        private bool _isInitialized = false;
        private BusConfig _config;

        private readonly ILogger<IBus> _logger;
        private string _consumerName;

        private IConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        public RabbitBus(ILogger<IBus> logger, IOptions<BusConfig> config)
        {
            _logger = logger;
            _config = config?.Value;
        }

        public Func<string> CorrelationIdFactory { get; set; }

        public IBus Init()
        {
            if(_isInitialized)
                throw new InvalidOperationException("Already initialized");

            if(_config == null)
                throw new InvalidOperationException("Config is null, can't initialize");

            return Init(_config.HostUri, _config.ConsumerName, _config.HeartbeatInterval,
                _config.AutomaticRecovery, _config.UserName, _config.Password, null);
        }

        public IBus Init(BusConfig config)
        {
            _config = config;
            return Init();
        }

        public RabbitBus Init(string uri, string consumerName, ushort heartbeat, bool reconnecting, string userName,
            string password, Func<string> correlationIdFactory)
        {
            _consumerName = string.IsNullOrWhiteSpace(consumerName) ? "unknown" : consumerName;
            CorrelationIdFactory = correlationIdFactory;

            _factory = new ConnectionFactory
            {
                Uri = new Uri(uri),
                RequestedHeartbeat = heartbeat,
                AutomaticRecoveryEnabled = reconnecting,
                UserName = userName ?? BusConfig.DEFAULT_USER_NAME,
                Password = password ?? BusConfig.DEFAULT_PASSWORD
            };

            InitPublishing();

            _isInitialized = true;
            return this;
        }

        public TMessage Message<TMessage>() where TMessage : IMessage
        {
            var message = (TMessage)Activator.CreateInstance(typeof(TMessage), this);
            return message;
        }

        public void Publish<TPayload>(IMessage message, TPayload payload) where TPayload : IPayload
        {
            if(!_isInitialized)
                throw new InvalidOperationException("Not initialized yet, " +
                                                    "don't forget to call 'Init()' method at least once");

            var wrapper = CreateWrapper(payload);
            var body = MessageSerializer.Serialize(wrapper);

            var channel = GetConnection().CreateModel();

            channel.BasicPublish(
                exchange: RabbitConsts.ExchangeName,
                routingKey: message.RoutingKey,           
                basicProperties: new BasicProperties()
                {
                    DeliveryMode = 2,
                    Persistent = true,
                    ContentType = "application/json",
                    ContentEncoding = "UTF-8",
                    AppId = RabbitConsts.QueuePrefix                   
                },
                body: body);
        }

        public void Subscribe<TPayload>(IMessage message, Func<IPayloadWrapper<TPayload>, Task<HandleResult>> handler, 
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers, SubscriptionConfig configuration = null)
            where TPayload : IPayload
        {
            var qName = InitializeQueue<TPayload>(message, type, configuration, out var channel, out _);

            var concurrencyLimit = configuration?.ConcurrencyLimit ?? _config.ConcurrencyLimit;

            var consumer = CreateConsumer(channel, handler);
            channel.BasicQos(0, (ushort)concurrencyLimit, false);
            channel.BasicConsume(queue: qName, autoAck: false, consumer: consumer);
        }

        public IPayloadWrapper<TPayload> Get<TPayload>(IMessage message,
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers)
            where TPayload : IPayload
        {
            var qName = InitializeQueue<TPayload>(message, type, null, out var channel, out _);

            var retrieved = channel.BasicGet(qName, true);
            if (retrieved == null)
                return null;

            if (TryDeserializeReceived(retrieved.Body, out PayloadWrapper<TPayload> deserialized))
                return deserialized;

            LogBadFormat(retrieved);
            return null;
        }

        public IPayloadWrapper<TPayload>[] GetAll<TPayload>(IMessage message,
            SubscriptionType type = SubscriptionType.SharedBetweenConsumers)
            where TPayload : IPayload
        {
            var result = new List<IPayloadWrapper<TPayload>>();
            var qName = InitializeQueue<TPayload>(message, type, null, out var channel, out _);

            while (true)
            {
                var retrieved = channel.BasicGet(qName, true);
                if (retrieved == null)
                    return result.ToArray();

                if (TryDeserializeReceived(retrieved.Body, out PayloadWrapper<TPayload> deserialized))
                {
                    result.Add(deserialized);
                    continue;
                }

                LogBadFormat(retrieved);
            }
        }


        ~RabbitBus()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // get rid of managed resources
            }

            // get rid of unmanaged resources
            _channel?.Dispose();
            _connection?.Dispose();
        }


        private string InitializeQueue<TPayload>(IMessage message, SubscriptionType type, SubscriptionConfig configuration,
            out IModel channel, out QueueDeclareOk declaration) where TPayload : IPayload
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Not initialized yet, " +
                                                    "don't forget to call 'Init()' method at least once");

            var consumerName = _consumerName;
            var autoDelete = false;

            if (type == SubscriptionType.PerConsumer)
            {
                autoDelete = true;
                consumerName = $"{consumerName}.{INSTANCE_IDENTIFICATION}";
            }

            var qName = RabbitConsts.GenerateQueueName(
                RabbitConsts.QueuePrefix,
                RabbitConsts.QueueConsumerPrefix,
                consumerName,
                message.RoutingKey);
            channel = GetChannel();

            var arguments = configuration?.QueueArguments;
            var createDeadLetter = configuration?.CreateDeadLetterQueue ?? _config.CreateDeadLetterQueue ?? false;
            if (createDeadLetter)
            {
                arguments = ConfigureDeadLetter(channel, qName, arguments);
            }

            declaration = channel.QueueDeclare(queue: qName, durable: true, exclusive: false, autoDelete: autoDelete, arguments: arguments);
            channel.QueueBind(queue: qName, exchange: RabbitConsts.ExchangeName, routingKey: message.RoutingKey);
            return qName;
        }

        private IPayloadWrapper<TPayload> CreateWrapper<TPayload>(TPayload payload) where TPayload : IPayload
        {
            var wrapper = new PayloadWrapper<TPayload>(
                payload, 
                DateTime.UtcNow,
                CorrelationIdFactory?.Invoke() ?? string.Empty
                );

            return wrapper;
        }

        private void InitPublishing()
        {
            GetChannel().ExchangeDeclare(
                exchange: RabbitConsts.ExchangeName,
                type: "topic",
                durable: true);
        }

        private IBasicConsumer CreateConsumer<TPayload>(IModel channel, Func<IPayloadWrapper<TPayload>, Task<HandleResult>> handler) where TPayload : IPayload
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, args) =>
            {
                try
                {
                    await HandleReceivedMessage(handler, args);
                }
                catch (Exception e)
                {
                    LogErrorMessage($"[MESSAGE HANDLING] Unexpected error while handling message, error: {e.Message}", e);
                }
            };
            return consumer;
        }

        private async Task HandleReceivedMessage<TPayload>(Func<IPayloadWrapper<TPayload>, Task<HandleResult>> handler,
                    BasicDeliverEventArgs args) where TPayload : IPayload
        {
            if (!TryDeserializeReceived(args.Body, out PayloadWrapper<TPayload> deserialized))
            {
                LogBadFormat(args);
                RemoveFromQueueAsReject(args);
                return;
            }

            try
            {
                var result = await handler(deserialized);

                switch (result.Type)
                {
                    case HandleType.Requeue:
                        ReturnToQueue(args);
                        break;
                    case HandleType.Reject:
                        RemoveFromQueueAsReject(args);
                        break;
                    case HandleType.Ok:
                    default:
                        RemoveFromQueueAsOk(args);
                        break;
                }

            }
            catch (PayloadMismatchException ex)
            {
                RemoveFromQueueAsOk(args);
                LogPayloadMismatch(ex);
                throw; // Fail fast and notify developer (implementation mistake)
            }
            catch (Exception ex)
            {
                ReturnToQueue(args);
                LogHandleException(ex);
            }
        }

        private void RemoveFromQueueAsOk(BasicDeliverEventArgs args)
        {
            GetChannel().BasicAck(args.DeliveryTag, false);
        }

        private void RemoveFromQueueAsReject(BasicDeliverEventArgs args)
        {
            GetChannel().BasicNack(args.DeliveryTag, false, false);
        }

        private void ReturnToQueue(BasicDeliverEventArgs args)
        {
            GetChannel().BasicNack(args.DeliveryTag, false, true);
        }

        private bool TryDeserializeReceived<TPayload>(byte[] body, out PayloadWrapper<TPayload> result) where TPayload : IPayload
        {
            try
            {
                result = MessageSerializer.Deserialize<PayloadWrapper<TPayload>>(body);

                return result != null;
            }
            catch (Exception ex)
            {
                LogDeserializingMessage(ex);
                result = null;
                return false;
            }
        }

        private IDictionary<string, object> ConfigureDeadLetter(IModel channel, string qName, IDictionary<string, object> arguments)
        {
            var argSafe = arguments ?? new Dictionary<string, object>();
            var qNameDead = $"{qName}.dead";

            // declare dead letter queue
            channel.QueueDeclare(queue: qNameDead, durable: true, exclusive: false,
                autoDelete: false);

            // connect with base queue via arguments
            if (!argSafe.ContainsKey("x-dead-letter-exchange"))
                argSafe["x-dead-letter-exchange"] = "";

            if (!argSafe.ContainsKey("x-dead-letter-routing-key"))
                argSafe["x-dead-letter-routing-key"] = qNameDead;

            return argSafe;
        }
        private IConnection GetConnection()
        {
            if (_connection != null && _connection.IsOpen)
                return _connection;
            _connection?.Dispose();
            _connection = _factory.CreateConnection(_consumerName);
            return _connection;
        }

        private IModel GetChannel()
        {
            if (_channel != null && _channel.IsOpen && !_channel.IsClosed)
                return _channel;
            _channel?.Dispose();
            var connection = GetConnection();
            _channel = connection.CreateModel();
            return _channel;
        }

        private void LogMessage(string msg)
        {
            var message = $"[RabbitMQ Bus] {msg}";
            if (_logger != null)
                _logger.LogWarning(message);
            else
                Debug.WriteLine(message);
        }

        private void LogErrorMessage(string msg, Exception ex)
        {
            var message = $"[KoBus] {msg}";
            if (_logger != null)
                _logger.LogError(ex, message);
            else
                Debug.WriteLine(message);
        }

        private void LogBadFormat(BasicDeliverEventArgs args)
        {
            LogMessage(
                $"[MESSAGE HANDLING] Received message is in bad format. Removing message from the queue. Exchange: {args.Exchange} | Routing: {args.RoutingKey} | Consumer: {args.ConsumerTag}");
        }

        private void LogBadFormat(BasicGetResult args)
        {
            LogMessage(
                $"[MESSAGE HANDLING] Retrieved message is in bad format. Removing message from the queue. Exchange: {args.Exchange} | Routing: {args.RoutingKey}");
        }

        private void LogDeserializingMessage(Exception ex)
        {
            LogMessage(
                $"[MESSAGE HANDLING] Exception while deserializing message: {ex}");
        }

        private void LogPayloadMismatch(PayloadMismatchException ex)
        {
            LogMessage($"[MESSAGE HANDLING] Payload mismatch, correct!. {ex.Message}");
        }

        private void LogHandleException(Exception ex)
        {
            LogMessage($"[MESSAGE HANDLING] Exception occured in handle method. Returning message to the queue. {ex}");
        }
    }
}
