namespace moabix.Repositories.Payments
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class PaymentRepoDI
    {
        public static void RegisterPaymentRepo(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPaymentsRepo, PaymentsRepo>();
        }
    }

    public class DynamoDBConfiguration
    {
        public string TableName { get; set; }

        public string Endpoint { get; set; }
    
    }
}
