using Microsoft.AspNetCore.Mvc;
using MovieRental.Movie;

namespace MovieRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {

        private readonly IMovieFeatures _features;

        public MovieController(IMovieFeatures features)
        {
            _features = features;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
	        return Ok(await _features.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Movie.Movie movie)
        {
	        return Ok(await _features.SaveAsync(movie));
        }
    }
}
