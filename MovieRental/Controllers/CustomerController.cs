using Microsoft.AspNetCore.Mvc;
using MovieRental.Customer;

namespace MovieRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerFeatures _customer;

        public CustomerController(ICustomerFeatures customer)
        {
            _customer = customer;
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int customerId)
        {
            return Ok(await _customer.GetByIdAsync(customerId));
        }

        [HttpPost]
        public async Task<IActionResult> Post(string customerName)
        {
            var customer = new Customer.Customer { Name = customerName };

            return Ok(await _customer.SaveAsync(customer));
        }
    }
}
