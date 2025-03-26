using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Infrastructure.RabbitMQ
{
    public class RabbitSender
    {
        private readonly RabbitConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitSender> _logger;
        private readonly string _exchangeName;
        private readonly string _routingKey;
        private readonly bool _durable;
        private readonly bool _exclusive;
        private readonly bool _autoDelete;

        // Constructor with Dependency Injection for Logger and configurable values
        public RabbitSender(RabbitConnectionFactory connectionFactory, ILogger<RabbitSender> logger,
            string exchangeName = "FlightDemoExchange", string routingKey = "flights-routing-key", bool durable = false, bool exclusive = false, bool autoDelete = false)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _exchangeName = exchangeName;
            _routingKey = routingKey;
            _durable = durable;
            _exclusive = exclusive;
            _autoDelete = autoDelete;
        }

        public async Task SendMessageAsync(string queueName, string message)
        {
            // Argument validation
            if (string.IsNullOrWhiteSpace(queueName))
            {
                _logger.LogError("Queue name is required.");
                throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogError("Message is required.");
                throw new ArgumentException("Message cannot be null or empty", nameof(message));
            }

            try
            {
                _logger.LogInformation($"Establishing connection to RabbitMQ server...");

                using (var connection = await _connectionFactory.CreateConnectionAsync())
                using (var channel = await _connectionFactory.CreateChannelAsync(connection))
                {
                    // _logger.LogInformation($"Declaring exchange: {_exchangeName} of type 'direct'...");
                    await channel.ExchangeDeclareAsync(_exchangeName, type: "direct");
                    // _logger.LogInformation($"Declaring queue: {queueName}...");
                    await channel.QueueDeclareAsync(queue: queueName, durable: _durable, exclusive: _exclusive, autoDelete: _autoDelete, arguments: null);
                    // _logger.LogInformation($"Binding queue {queueName} to exchange {_exchangeName} with routing key {_routingKey}...");
                    await channel.QueueBindAsync(queueName, _exchangeName, _routingKey, null);

                    var body = Encoding.UTF8.GetBytes(message);
                    _logger.LogInformation($"Publishing message to exchange {_exchangeName} with routing key {_routingKey}...");

                    await channel.BasicPublishAsync(exchange: _exchangeName, routingKey: _routingKey, body: body);

                    _logger.LogInformation($"Message successfully sent to queue {queueName}.");
                }
            }
            catch (Exception ex)
            {
                // Log the error with detailed information
                _logger.LogError(ex, $"An error occurred while sending message to queue {queueName}: {ex.Message}");
                throw new InvalidOperationException($"Error sending message to RabbitMQ: {ex.Message}", ex);
            }
        }
    }
}
