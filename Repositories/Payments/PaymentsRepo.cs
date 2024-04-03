using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using moabix.Models;
using moabix.Services.PaymentsQueue.DI;

namespace moabix.Repositories.Payments
{
    public class PaymentsRepo : IPaymentsRepo
    {
        private IAmazonDynamoDB _client;
        private DynamoDBContext _context;
        private readonly string? _tableName = null;
        private readonly IConfiguration _configuration;

        public PaymentsRepo(IConfiguration configuration)
        {
            var dynamoDBConfig = configuration.GetSection("DynamoDB").Get<DynamoDBConfiguration>();

            _tableName = dynamoDBConfig.TableName;
            var authKeys = configuration.GetSection("AWSAUTH").Get<AWSAUTH>();
            var credentials = new BasicAWSCredentials(authKeys.accessKey, authKeys.secretKey);
            var client = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);
            _client = client;
            _context = new DynamoDBContext(client);
        }

        public async Task SavePaymentAsync(Payment payment)
        {
            try
            {
                await _context.SaveAsync(payment);
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        // Implement additional repository methods for CRUD operations
    }
}
