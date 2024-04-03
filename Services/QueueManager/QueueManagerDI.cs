namespace moabix.Services.QueueManager
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using moabix.Repositories.Payments;

    public static class RabbitMQServiceRegistration
    {
        public static void RegisterRabbitMQService(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQConfiguration>();

            Console.WriteLine(rabbitMQConfig.ToString());

            services.AddTransient<IPaymentsManager, PaymentsManager>(provider =>
            {
                var IPaymentsRepo = provider.GetService<IPaymentsRepo>();
                return new PaymentsManager(rabbitMQConfig.HostName, rabbitMQConfig.QueueName, IPaymentsRepo);
            });
        }
    }

    public class RabbitMQConfiguration
    {
        public string HostName { get; set; }
        public string QueueName { get; set; }
    }
}
