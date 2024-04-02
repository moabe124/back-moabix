namespace moabix.QueueManager
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class RabbitMQServiceRegistration
    {
        public static void RegisterRabbitMQService(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQConfiguration>();

            Console.WriteLine(rabbitMQConfig.ToString());

            services.AddTransient<IRabbitManager, RabbitManager>(provider =>
            {
                return new RabbitManager(rabbitMQConfig.HostName, rabbitMQConfig.QueueName);
            });
        }
    }

    public class RabbitMQConfiguration
    {
        public string HostName { get; set; }
        public string QueueName { get; set; }
    }
}
