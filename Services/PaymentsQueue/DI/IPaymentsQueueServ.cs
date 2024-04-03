using moabix.Models;

namespace moabix.Services.PaymentsQueue.DI
{
    public interface IPaymentsQueueServ
    {
        void PublishPayment(Payment payment);

        void StartMessageConsumer();
        void CleanPaymentQueue();
    }
}
