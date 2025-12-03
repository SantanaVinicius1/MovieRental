using MovieRental.Contracts;

namespace MovieRental.Rental;

public interface IRentalFeatures
{
	Task<Rental> SaveAsync(CreateRentalRequest rental);
	Task<IEnumerable<Rental>> GetRentalsByCustomerAsync(int customerId);
}