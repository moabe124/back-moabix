using Amazon.DynamoDBv2.DataModel;

namespace moabix.Models
{
    [DynamoDBTable("moabix_test")]
    public class Payment
    {

        [DynamoDBHashKey("PaymentId")]
        public string PaymentId { get; set; }
        [DynamoDBProperty]
        public decimal Amount { get; set; }
        [DynamoDBRangeKey]
        public DateTime UpdatedAt { get; set; }
        [DynamoDBProperty]
        public DateTime CreatedAt {  get; set; }
        [DynamoDBProperty]
        public string? Description { get; set; }

        // Additional properties can be added as needed, such as payer information, payment method, etc.

        public override string ToString()
        {
            return $"Payment ID: {PaymentId}, Amount: {Amount}, Timestamp: {UpdatedAt}, Description: {Description}";
        }
    }
}
