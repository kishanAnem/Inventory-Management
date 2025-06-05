using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryManagement.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InventoryManagement.Infrastructure.Services
{
    public class RabbitMQMessageBroker : IMessageBroker, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IConfiguration _configuration;
        private bool _disposed;

        public RabbitMQMessageBroker(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"],
                UserName = _configuration["RabbitMQ:Username"],
                Password = _configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public bool IsConnected => _connection?.IsOpen ?? false;

        public Task ConnectAsync()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("RabbitMQ connection failed");
            }
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            Dispose();
            return Task.CompletedTask;
        }

        public Task PublishAsync<T>(string topic, T message) where T : class
        {
            _channel.ExchangeDeclare(topic, ExchangeType.Topic, durable: true);
            
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(
                exchange: topic,
                routingKey: topic,
                basicProperties: null,
                body: body);

            return Task.CompletedTask;
        }

        public Task SubscribeAsync<T>(string topic, Func<T, Task> handler) where T : class
        {
            _channel.ExchangeDeclare(topic, ExchangeType.Topic, durable: true);
            
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, topic, topic);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
                if (message != null)
                {
                    await handler(message);
                }
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: queueName,
                                autoAck: false,
                                consumer: consumer);

            return Task.CompletedTask;
        }

        public Task UnsubscribeAsync(string topic)
        {
            _channel.ExchangeDelete(topic);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _channel?.Dispose();
                _connection?.Dispose();
            }

            _disposed = true;
        }
    }
}
