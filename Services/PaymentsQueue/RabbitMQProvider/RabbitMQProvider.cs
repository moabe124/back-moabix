using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using moabix.Models;
using moabix.Repositories.Payments;
using moabix.Services.PaymentsQueue.DI;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace moabix.Services.PaymentsQueue.RabbitMQ
{
    public class RabbitMQProvider : IPaymentsQueueServ
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;
        private IPaymentsRepo _paymentsRepo;

        public RabbitMQProvider(IPaymentsRepo paymentsRepo, IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQConfiguration>();
            _factory = new ConnectionFactory();
            // TODO pass user and password to user secrets, default will be admin/admin
            _factory.Uri = new Uri($"amqp://admin:admin@{rabbitMQConfig.HostName}:5672/");
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = rabbitMQConfig.QueueName;
            _paymentsRepo = paymentsRepo;

            ConsumeMessages();
        }

        public void PublishPayment(Payment payment)
        {
            try
            {
                _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

                var paymentJson = JsonConvert.SerializeObject(payment);
                var body = Encoding.UTF8.GetBytes(paymentJson);

                _channel.BasicPublish(exchange: "",
                                     routingKey: _queueName,
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine($"[x] Sent {paymentJson}");
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        private void ConsumeMessages()
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
                var paymentJson = Encoding.UTF8.GetString(body);
                var payment = JsonConvert.DeserializeObject<Payment>(paymentJson);
                Console.WriteLine($"[x] Received {paymentJson}");

                // Save the payment
                _paymentsRepo.SavePaymentAsync(payment);

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

        public void CleanPaymentQueue()
        {
            _channel.QueuePurge(_queueName);
        }

        public void StartMessageConsumer()
        {
            throw new NotImplementedException();
        }
    }
}
