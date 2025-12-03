
namespace MovieRental.PaymentProviders
{
    public class FaultyPaymentProvider : IPaymentProvider
    {
        public Task<bool> Pay(double price)
        {
            //this is simulates a failed payment
            return Task.FromResult(false);
        }
    }
}
