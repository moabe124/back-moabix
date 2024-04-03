using moabix.Models;

namespace moabix.Repositories.Payments
{
    public interface IPaymentsRepo
    {
        public Task SavePaymentAsync(Payment payment);
    }
}
