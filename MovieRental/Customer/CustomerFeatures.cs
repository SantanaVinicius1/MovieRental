
using Microsoft.EntityFrameworkCore;
using MovieRental.Data;

namespace MovieRental.Customer
{
    public class CustomerFeatures : ICustomerFeatures
    {
        private readonly MovieRentalDbContext _movieRentalDb;
        public CustomerFeatures(MovieRentalDbContext movieRentalDb)
        {
            _movieRentalDb = movieRentalDb;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _movieRentalDb.Customers
                .AsNoTracking()
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Customer> SaveAsync(Customer customer)
        {
            _movieRentalDb.Customers.Add(customer);
            await _movieRentalDb.SaveChangesAsync();

            return customer;
        }
    }
}
