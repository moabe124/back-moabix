using moabix.Services.QueueManager;

namespace moabix.Repositories.Payments
{
    using Amazon;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DataModel;
    using Amazon.Runtime;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class PaymentRepoDI
    {
        public static void RegisterPaymentRepo(this IServiceCollection services, IConfiguration configuration)
        {
            var dynamoDBConfig = configuration.GetSection("DynamoDB").Get<DynamoDBConfiguration>();

            Console.WriteLine(dynamoDBConfig.ToString());

            services.AddTransient<IPaymentsRepo, PaymentsRepo>(provider =>
            {
                return new PaymentsRepo(dynamoDBConfig.TableName);
            });
        }
    }

    public class DynamoDBConfiguration
    {
        public string TableName { get; set; }

        public string Endpoint { get; set; }
    
    }
}
