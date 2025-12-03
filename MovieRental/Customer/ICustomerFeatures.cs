namespace MovieRental.Customer
{
    public interface ICustomerFeatures
    {
        Task<Customer> SaveAsync(Customer customer);
        Task<Customer> GetByIdAsync(int id);
    }
}
