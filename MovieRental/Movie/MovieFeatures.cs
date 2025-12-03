using Microsoft.EntityFrameworkCore;
using MovieRental.Data;

namespace MovieRental.Movie
{
	public class MovieFeatures : IMovieFeatures
	{
		private readonly MovieRentalDbContext _movieRentalDb;
		public MovieFeatures(MovieRentalDbContext movieRentalDb)
		{
			_movieRentalDb = movieRentalDb;
		}
		
		public async Task<Movie> SaveAsync(Movie movie)
		{
			_movieRentalDb.Movies.Add(movie);
			_movieRentalDb.SaveChanges();
			return movie;
		}

		public async Task<IEnumerable<Movie>> GetAllAsync()
		{
			return await _movieRentalDb
				.Movies
				.AsNoTracking()
				.ToListAsync();
		}
	}
}
