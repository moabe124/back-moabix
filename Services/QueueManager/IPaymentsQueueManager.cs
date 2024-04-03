using moabix.Models;

namespace moabix.Services.QueueManager
{
    public interface IPaymentsQueueManager
    {
        void PublishPayment(Payment payment);

        void CleanPaymentQueue();
    }
}
