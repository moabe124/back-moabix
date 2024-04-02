using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace moabix.QueueManager
{
    public class RabbitManager : IRabbitManager
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;

        public RabbitManager(string hostName, string queueName)
        {
            _factory = new ConnectionFactory() { HostName = hostName };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = queueName;
        }

        public void PublishMessage(string message)
        {
            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($"[x] Sent {message}");
        }

        public void ConsumeMessages()
        {
            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received {message}");

                // Process the message (e.g., invoke appropriate handler)
                // ...

                Thread.Sleep(1000); // Simulate processing time

                Console.WriteLine("[x] Done");

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _queueName,
                                 autoAck: false,
                                 consumer: consumer);

            Console.WriteLine("Waiting for messages...");
        }

        public void CloseConnection()
        {
            _connection.Close();
        }
    }
}
