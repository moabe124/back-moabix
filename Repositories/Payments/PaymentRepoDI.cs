namespace moabix.Repositories.Payments
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class PaymentRepoDI
    {
        public static void AddPaymentRepo(this IServiceCollection services)
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
