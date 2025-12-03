using Microsoft.AspNetCore.Mvc;
using MovieRental.Contracts;
using MovieRental.Movie;
using MovieRental.Rental;

namespace MovieRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RentalController : ControllerBase
    {

        private readonly IRentalFeatures _features;

        public RentalController(IRentalFeatures features)
        {
            _features = features;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateRentalRequest rental)
        {
	        return Ok(await _features.SaveAsync(rental));
        }

        [HttpGet]
        public async Task<IActionResult> Get(int customerId)
        {
            return Ok(await _features.GetRentalsByCustomerAsync(customerId));
        }

	}
}
