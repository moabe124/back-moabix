using Microsoft.AspNetCore.Connections;
using moabix.Models;
using moabix.Repositories.Payments;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace moabix.Services.QueueManager
{
    public class PaymentsManager : IPaymentsManager
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;
        private IPaymentsRepo _paymentsRepo;

        public PaymentsManager(string hostName, string queueName, IPaymentsRepo paymentsRepo)
        {
            _factory = new ConnectionFactory();
            // TODO pass user and password to user secrets, default will be admin/admin
            _factory.Uri = new Uri($"amqp://admin:admin@{hostName}:5672/");
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = queueName;
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
    }
}
