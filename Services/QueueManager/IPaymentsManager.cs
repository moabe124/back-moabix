using moabix.Models;

namespace moabix.Services.QueueManager
{
    public interface IPaymentsManager
    {
        void PublishPayment(Payment payment);

        void CleanPaymentQueue();
    }
}
