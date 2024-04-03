using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using moabix.Models;

namespace moabix.Repositories.Payments
{
    public class PaymentsRepo : IPaymentsRepo
    {
        private IAmazonDynamoDB _client;
        private DynamoDBContext _context;
        private readonly string? _tableName = null;

        public PaymentsRepo(string tableName)
        {
            _tableName = tableName;
            // TODO pass those credentials to user secrets
            var credentials = new BasicAWSCredentials();
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
