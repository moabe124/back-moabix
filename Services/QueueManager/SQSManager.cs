using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SQS;
using Amazon.SQS.Model;
using moabix.Models;
using moabix.Repositories.Payments;
using Newtonsoft.Json;
using System.Text;

namespace moabix.Services.QueueManager
{
    public class SQSManager : IPaymentsQueueManager
    {
        private readonly IConfiguration _configuration;
        public AmazonSQSClient _client;
        public IPaymentsRepo _paymentsRepo;
        public string _SQSQueueUrl;

        public SQSManager(IPaymentsRepo paymentsRepo, IConfiguration configuration)
        {
            var authKeys = configuration.GetSection("AWSAUTH").Get<AWSAUTH>();
            var credentials = new BasicAWSCredentials(authKeys.accessKey, authKeys.secretKey);
            _client = new AmazonSQSClient(credentials, RegionEndpoint.USEast1);
            _SQSQueueUrl = configuration.GetSection("SQS").Get<SQSConfiguration>().QueueUrl;
            _paymentsRepo = paymentsRepo;
            ConsumeMessages();
            _configuration = configuration;
        }

        public void CleanPaymentQueue()
        {
            throw new NotImplementedException();
        }

        public async void PublishPayment(Payment payment)
        {
            var paymentJson = JsonConvert.SerializeObject(payment);
            var body = Encoding.UTF8.GetBytes(paymentJson);

            var request = new SendMessageRequest
            {
                QueueUrl = _SQSQueueUrl,
                MessageBody = paymentJson
            };

            var response = await _client.SendMessageAsync(request);

            Console.WriteLine($"Message sent successfully. MessageId: {response.MessageId}");
        }

        public async void ConsumeMessages()
        {
            var request = new ReceiveMessageRequest
            {
                QueueUrl = _SQSQueueUrl,
                WaitTimeSeconds = 20 // Long-polling timeout (seconds)
            };

            var response = await _client.ReceiveMessageAsync(request);

            if (response.Messages.Count > 0)
            {
                foreach (var message in response.Messages)
                {
                    Console.WriteLine($"Received message: {message.Body}");

                    // Process the message
                    var payment = JsonConvert.DeserializeObject<Payment>(message.Body);

                    // Delete the message from the queue
                    var deleteRequest = new DeleteMessageRequest
                    {
                        QueueUrl = _SQSQueueUrl,
                        ReceiptHandle = message.ReceiptHandle
                    };
                    await _client.DeleteMessageAsync(deleteRequest);
                }
            }
        }
    }
}
