using Microsoft.EntityFrameworkCore;
using MovieRental.Contracts;
using MovieRental.Customer;
using MovieRental.Data;
using MovieRental.PaymentProviders;

namespace MovieRental.Rental
{
	public class RentalFeatures : IRentalFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		private readonly IKeyedServiceProvider _paymentsProvider;

		public RentalFeatures(MovieRentalDbContext movieRentalDb, IKeyedServiceProvider paymentsProvider)
		{
			_movieRentalDb = movieRentalDb;
			_paymentsProvider = paymentsProvider;
		}

		public async Task<Rental> SaveAsync(CreateRentalRequest request)
		{
			var rental = new Rental()
			{
				MovieId = request.MovieId,
				CustomerId = request.CustomerId,
				DaysRented = request.DaysRented,
				PaymentMethod = request.PaymentMethod,
			};

			var pProvider = _paymentsProvider.GetRequiredKeyedService<IPaymentProvider>(rental.PaymentMethod);

			if(await pProvider.Pay(10))
			{
                _movieRentalDb.Rentals.Add(rental);
                await _movieRentalDb.SaveChangesAsync();
                return rental;
            }

			return null;
		}

		public async Task<IEnumerable<Rental>> GetRentalsByCustomerAsync(int customerId)
		{
			return await _movieRentalDb.Rentals
				.Include(r => r.Movie)
				.Include(r => r.Customer)
				.AsNoTracking()
				.Where(r => r.CustomerId == customerId)
				.ToListAsync();
        }
	}
}
