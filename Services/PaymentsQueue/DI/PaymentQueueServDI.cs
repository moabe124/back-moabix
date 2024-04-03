namespace moabix.Services.PaymentsQueue.DI
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using moabix.Services.PaymentsQueue.RabbitMQ;
    using moabix.Services.PaymentsQueue.SQSProvider;

    public static class RabbitMQServiceRegistration
    {
        public static void AddPaymentQueueServ(this IServiceCollection services, IConfiguration configuration)
        {

            var queueProvider = configuration["QueueProvider"];

            if (queueProvider == null)
            {
                throw new Exception("Queue provider não configurado.");
            }

            if (queueProvider.Equals(QueueType.SQS.ToString()))
            {
                services.AddSingleton<IPaymentsQueueServ, SQSManager>();

            }
            else if (queueProvider.Equals(QueueType.RABBITMQ.ToString()))
            {
                services.AddTransient<IPaymentsQueueServ, RabbitMQProvider>();
            }

        }
    }

    public enum QueueType
    {
        SQS,
        RABBITMQ
    }

    public class RabbitMQConfiguration
    {
        public string HostName { get; set; }
        public string QueueName { get; set; }
    }

    public class SQSConfiguration
    {
        public string QueueUrl { get; set; }
    }

    public class AWSAUTH
    {
        public string accessKey { get; set; }
        public string secretKey { get; set; }
    }
}
